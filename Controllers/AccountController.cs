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

        // Konstruktori i kontrollerit që injekton varësitë e nevojshme
        public AccountController(AppDbContext db, AuthService auth)
        {
            _db = db;
            _auth = auth;
        }

        // Metodë GET për të shfaqur formën e login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // Metodë POST për të përpunuar të dhënat e login
        [HttpPost]
        [ValidateAntiForgeryToken] // Mbrojtje kundër CSRF
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            // Kontrollon nëse modeli është i vlefshëm
            if (!ModelState.IsValid)
                return View(model);

            // Kërkon përdoruesin me email-in e dhënë
            var user = _db.Users.FirstOrDefault(u => u.Email == model.Email);
            
            // Verifikon fjalëkalimin (u rregullua rendi i parametrave)
            if (user == null || !_auth.VerifyPassword(user.PasswordHash, model.Password))
            {
                ModelState.AddModelError("", "Invalid email or password");
                return View(model);
            }

            // Krijon claims për autentifikim
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var claimsIdentity = new ClaimsIdentity(claims, 
                CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            // Kryen hyrjen në sistem
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme, 
                claimsPrincipal,
                new AuthenticationProperties
                {
                    IsPersistent = true // Opsionale: mbaj mend mua
                });

            // Ridrejton tek faqja kryesore e kredit kartelave
            return RedirectToAction("Index", "CreditCards");
        }

        // Metodë GET për të shfaqur formën e regjistrimit
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // Metodë POST për të përpunuar të dhënat e regjistrimit
        [HttpPost]
        [ValidateAntiForgeryToken] // Mbrojtje kundër CSRF
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Kontrollon nëse përdoruesi ekziston tashmë (u rregullua: përdor _db në vend të _dbContext)
                if (_db.Users.Any(u => u.Email == model.Email))
                {
                    ModelState.AddModelError("Email", "Email already registered.");
                    return View(model);
                }

                // Krijon përdorues të ri me fjalëkalim të hash-uar (u rregullua: përdor _auth në vend të _authService)
                var user = new User
                {
                    Email = model.Email,
                    PasswordHash = _auth.HashPassword(model.Password)
                };

                _db.Users.Add(user);
                await _db.SaveChangesAsync(); // U shtua await

                // Hyn automatikisht pas regjistrimit
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

        // Metodë POST për daljen nga sistemi
        [HttpPost]
        [ValidateAntiForgeryToken] // Mbrojtje kundër CSRF
        public async Task<IActionResult> Logout()
        {
            // Shkyç përdoruesin
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}