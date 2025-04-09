using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using QRCodeBasedMetroTicketingSystem.Application.Interfaces.Services;
using QRCodeBasedMetroTicketingSystem.Web.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Razor;
using QRCodeBasedMetroTicketingSystem.Application.Common.Result;

namespace QRCodeBasedMetroTicketingSystem.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;
        private readonly IRazorViewEngine _viewEngine;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly IServiceProvider _serviceProvider;
        private readonly IEmailService _emailService;

        public AccountController(IUserService userService, 
            ITokenService tokenService,
            IRazorViewEngine viewEngine,
            ITempDataProvider tempDataProvider,
            IServiceProvider serviceProvider,
            IEmailService emailService)
        {
            _userService = userService;
            _tokenService = tokenService;
            _viewEngine = viewEngine;
            _tempDataProvider = tempDataProvider;
            _serviceProvider = serviceProvider;
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
                await SendEmailVerificationMail(model.Email, model.FullName);
                TempData["RegisteredEmail"] = model.Email;
                return RedirectToAction("VerificationEmailSent");
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
                    await SendEmailVerificationMail(result.User.Email, result.User.FullName);
                    TempData["RegisteredEmail"] = result.User.Email;
                    TempData["InfoMessage"] = "Please verify your email before logging in.";
                    return RedirectToAction("VerificationEmailSent");
                }

                // Create claims from JWT token data
                var tokenHandler = new JwtSecurityTokenHandler();
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, result.User.Id.ToString()),
                    new Claim(ClaimTypes.Name, result.User.FullName!),
                    new Claim(ClaimTypes.Role, "User"),
                    new Claim("JwtToken", result.Token)
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

                Response.Cookies.Append("jwtToken", result.Token, new CookieOptions
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
            Response.Cookies.Delete("jwtToken");

            return RedirectToAction("Index", "Home");
        }

        [Route("VerificationEmailSent")]
        public IActionResult VerificationEmailSent()
        {
            var email = TempData["RegisteredEmail"]?.ToString();
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
            await SendEmailVerificationMail(user.Email, user.FullName);

            return Json(new { success = true, message = "Verification email has been resent. Please check your inbox." });
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
                TempData["VerificationStatus"] = "Success";
                return RedirectToAction("EmailVerificationSuccess", "Account");
            }
            else
            {
                TempData["VerificationStatus"] = "Failed";
                return RedirectToAction("EmailVerificationFailed", "Account");
            }     
        }

        [Route("EmailVerificationSuccess")]
        public IActionResult EmailVerificationSuccess()
        {
            if (TempData["VerificationStatus"]?.ToString() != "Success")
                return RedirectToAction("Login", "Account");

            return View();
        }

        [Route("EmailVerificationFailed")]
        public IActionResult EmailVerificationFailed()
        {
            if (TempData["VerificationStatus"]?.ToString() != "Failed")
                return RedirectToAction("Login", "Account");

            return View();
        }

        private async Task SendEmailVerificationMail(string email, string name)
        {
            // Generate verification token (valid for 24 hours)
            string token = await _tokenService.GenerateEmailVerificationToken(email);

            // Generate verification URL
            var verificationUrl = Url.Action("VerifyEmail", "Account", new { email = email, token = token }, Request.Scheme);

            // Send verification email
            await _emailService.SendEmailVerificationAsync(email, name, verificationUrl!);
        }
    }
}
