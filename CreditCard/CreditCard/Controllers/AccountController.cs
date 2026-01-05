using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using CreditCard.Data;
using CreditCard.Services;
using CreditCard.ViewModels;
using CreditCard.Models;

namespace CreditCard.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _db;
        private readonly AuthService _auth;

        public AccountController(AppDbContext db, AuthService auth)
        {
            _db = db;
            _auth = auth;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = _db.Users.FirstOrDefault(u => u.Email == model.Email);
            
            // FIXED: Correct parameter order - (passwordHash, password)
            if (user == null || !_auth.VerifyPassword(user.PasswordHash, model.Password))
            {
                ModelState.AddModelError("", "Invalid email or password");
                return View(model);
            }

            // Create claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var claimsIdentity = new ClaimsIdentity(claims, 
                CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            // Sign in
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme, 
                claimsPrincipal,
                new AuthenticationProperties
                {
                    IsPersistent = true // Optional: remember me
                });

            // Redirect to CreditCards/Index
            return RedirectToAction("Index", "CreditCards");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Check if user already exists - FIXED: using _db instead of _dbContext
                if (_db.Users.Any(u => u.Email == model.Email))
                {
                    ModelState.AddModelError("Email", "Email already registered.");
                    return View(model);
                }

                // Create new user with hashed password - FIXED: using _auth instead of _authService
                var user = new User
                {
                    Email = model.Email,
                    PasswordHash = _auth.HashPassword(model.Password)
                };

                _db.Users.Add(user);
                await _db.SaveChangesAsync(); // FIXED: Added await

                // Automatically log in after registration
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email)
                };

                var claimsIdentity = new ClaimsIdentity(claims, 
                    CookieAuthenticationDefaults.AuthenticationScheme);
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme, 
                    claimsPrincipal);

                return RedirectToAction("Login", "");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}