using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using QRCodeBasedMetroTicketingSystem.Application.Interfaces.Services;
using QRCodeBasedMetroTicketingSystem.Web.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace QRCodeBasedMetroTicketingSystem.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;

        public AccountController(IUserService userService)
        {
            _userService = userService;
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

            if (await _userService.CheckPhoneExistsAsync(model.Phone))
            {
                ModelState.AddModelError("Phone", "Phone number is already registered");
                return View(model);
            }

            var result = await _userService.RegisterUserAsync(model);

            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = result.Message;
                return RedirectToAction("Login");
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

            var result = await _userService.LoginUserAsync(model.Phone, model.Password);
            if (result.IsSuccess)
            {
                // Create claims from JWT token data
                var tokenHandler = new JwtSecurityTokenHandler();

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, result.UserId.ToString()),
                    new Claim(ClaimTypes.Name, result.FullName),
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
    }
}
