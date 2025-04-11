using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using QRCodeBasedMetroTicketingSystem.Application.Interfaces.Services;
using QRCodeBasedMetroTicketingSystem.Web.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using QRCodeBasedMetroTicketingSystem.Application.DTOs;

namespace QRCodeBasedMetroTicketingSystem.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;

        public AccountController(IUserService userService, ITokenService tokenService, IEmailService emailService)
        {
            _userService = userService;
            _tokenService = tokenService;
            _emailService = emailService;
        }

        [Route("Register")]
        public IActionResult Register()
        {
            if (User.Identity?.IsAuthenticated == true && User.IsInRole("User"))
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost("Register")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (await _userService.CheckEmailExistsAsync(model.Email))
            {
                ModelState.AddModelError("Email", "Email is already registered");
                return View(model);
            }

            if (await _userService.CheckPhoneExistsAsync(model.PhoneNumber))
            {
                ModelState.AddModelError("Phone", "Phone number is already registered");
                return View(model);
            }

            var result = await _userService.RegisterUserAsync(model);

            if (result.IsSuccess)
            {
                var isSent = await SendEmailVerificationMail(model.Email, model.FullName);
                if (isSent)
                {
                    return RedirectToAction("VerificationEmailSent", new { email = model.Email });
                }
                else
                {
                    TempData["InfoMessage"] = "Account created, but failed to send verification email. Please login to send again.";
                    return RedirectToAction("Login", "Account");
                }
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
                return View(model);
            }
        }

        [Route("Login")]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true && User.IsInRole("User"))
            {
                return RedirectToAction("Index", "Home");
            }

            if(!Url.IsLocalUrl(returnUrl))
            {
                returnUrl = Url.Content("~/");
            }

            ViewData["ReturnUrl"] = returnUrl;

            return View();
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(UserLoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _userService.LoginUserAsync(model.PhoneNumber, model.Password);
            if (result.IsSuccess)
            {
                // Check if email is verified
                if (!result.User.IsEmailVerified)
                {
                    var isSent = await SendEmailVerificationMail(result.User.Email, result.User.FullName);
                    if (isSent)
                    {
                        TempData["InfoMessage"] = "Please verify your email before logging in.";
                        return RedirectToAction("VerificationEmailSent", new { email = result.User.Email });
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "An error occurred while attempting to log in. Please try again.";
                        return View(model);
                    }
                }

                // Create claims from JWT token data
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, result.User.Id.ToString()),
                    new Claim(ClaimTypes.Name, result.User.FullName!),
                    new Claim(ClaimTypes.Role, ApplicationRoles.User),
                    new Claim(CustomClaimTypes.JwtToken, result.Token)
                };

                // Sign in using cookie authentication
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal,
                    new AuthenticationProperties
                    {
                        IsPersistent = model.RememberMe,
                        ExpiresUtc = DateTime.UtcNow.AddDays(30)
                    });

                Response.Cookies.Append(CookieNames.JwtToken, result.Token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddHours(1)
                });

                return LocalRedirect(returnUrl ?? Url.Content("~/"));
            }
            else
            {
                ModelState.AddModelError("Password", result.Message);
                return View(model);
            }
        }

        [Route("Logout")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Logout()
        {
            // Sign out of cookie authentication
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Clear the JWT token cookie
            Response.Cookies.Delete(CookieNames.JwtToken);

            return RedirectToAction("Index", "Home");
        }

        [Route("VerificationEmailSent")]
        public IActionResult VerificationEmailSent(string email)
        {
            if (string.IsNullOrEmpty(email))
                return RedirectToAction("Login", "Account");

            return View();
        }

        [HttpPost("ResendVerificationEmail")]
        public async Task<IActionResult> ResendVerificationEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return Json(new { success = false, message = "Email address is required." });
            }

            // Check if user exists and is not verified
            var user = await _userService.GetUserByEmailAsync(email);
            if (user == null)
            {
                return Json(new { success = false, message = "User not found." });
            }

            if (user.IsEmailVerified)
            {
                return Json(new { success = false, message = "Email is already verified." });
            }

            // Send verification email
            var isSent = await SendEmailVerificationMail(user.Email, user.FullName);
            if (isSent)
            {
                return Json(new { success = true, message = "Verification email has been resent. Please check your inbox." });
            }
            else
            {
                Response.StatusCode = 500;
                return Json(new
                {
                    success = false,
                    message = "An error occurred while sending the email. Please try again."
                });
            }
        }

        [Route("VerifyEmail")]
        public async Task<IActionResult> VerifyEmail(string email, string token)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
            {
                return RedirectToAction("EmailVerificationFailed", "Account");
            }

            var result = await _userService.VerifyEmailAsync(email, token);

            if (result.IsSuccess)
            {
                return RedirectToAction("EmailVerificationSuccess", "Account", new { token });
            }
            else
            {
                return RedirectToAction("EmailVerificationFailed", "Account", new { token });
            }     
        }

        [Route("EmailVerificationSuccess")]
        public IActionResult EmailVerificationSuccess(string token)
        {
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Account");

            return View();
        }

        [Route("EmailVerificationFailed")]
        public IActionResult EmailVerificationFailed(string token)
        {
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Account");

            return View();
        }

        [Route("ForgotPassword")]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost("ForgotPassword")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userService.GetUserByEmailAsync(model.Email);
            if (user == null)
            {
                // Not reveal that the user does not exist
                return RedirectToAction("ForgotPasswordConfirmation", new { email = model.Email });
            }

            // Send password reset email and check
            var isSent = await SendPasswordResetMail(user.Email, user.FullName);
            if (!isSent)
            {
                TempData["ErroeMessage"] = "An error occurred while sending the email. Please try again.";
                return View(model);
            }

            return RedirectToAction("ForgotPasswordConfirmation", new { email = model.Email });
        }

        [Route("ForgotPasswordConfirmation")]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [Route("ResetPassword")]
        public IActionResult ResetPassword(string email, string token)
        {
            var viewModel = new ResetPasswordModel
            {
                Email = email,
                Token = token
            };
            return View(viewModel);
        }

        [HttpPost("ResetPassword")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userService.GetUserByEmailAsync(model.Email);
            var result = await _userService.ResetPassword(model);

            if (result.IsSuccess)
            {
                if (user != null)
                {
                    var bdTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Bangladesh Standard Time");
                    var bdTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, bdTimeZone);

                    var emailModel = new PasswordResetConfirmationEmailModel
                    {
                        FullName = user.FullName,
                        Email = user.Email,
                        DateTime = bdTime.ToString("f"),
                    };

                    // Send reset password confirmation email
                    await _emailService.SendPasswordResetConfirmationMail(emailModel);
                }
                
                return RedirectToAction("ResetPasswordConfirmation", new {email = model.Email});
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
                return View(model);
            }
        }

        [Route("ResetPasswordConfirmation")]
        public IActionResult ResetPasswordConfirmation(string email)
        {
            if (string.IsNullOrEmpty(email))
                return RedirectToAction("Login", "Account");

            return View();
        }

        private async Task<bool> SendEmailVerificationMail(string email, string name)
        {
            // Generate verification token (valid for 24 hours)
            string token = await _tokenService.GenerateEmailVerificationToken(email);

            // Generate verification URL
            var verificationUrl = Url.Action("VerifyEmail", "Account", new { email, token }, Request.Scheme);

            // Send verification email and return result
            return await _emailService.SendEmailVerificationAsync(email, name, verificationUrl!);
        }

        private async Task<bool> SendPasswordResetMail(string email, string name)
        {
            // Generate passeord reset token (valid for 30 minutes)
            string token = await _tokenService.GeneratePasswordResetToken(email);

            // Generate passeord reset URL
            var resetUrl = Url.Action("ResetPassword", "Account", new { email, token }, Request.Scheme);

            // Send passeord reset email
            return await _emailService.SendPasswordResetEmailAsync(email, name, resetUrl!);
        }
    }
}
