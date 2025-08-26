using Microsoft.AspNetCore.Mvc;
using WebApplication_ShopItemGame.Data;
using WebApplication_ShopItemGame.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace WebApplication_ShopItemGame.Controllers
{
    public class AuthController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly PasswordHasher<User> _passwordHasher;

        public AuthController(ApplicationDbContext db)
        {
            _db = db;
            _passwordHasher = new PasswordHasher<User>();
        }

        // GET: /Auth/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Auth/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {
                if (_db.Users.Any(u => u.Username == user.Username))
                {
                    ModelState.AddModelError("Username", "Username already exists");
                    return View(user);
                }
                if (_db.Users.Any(u => u.Email == user.Email))
                {
                    ModelState.AddModelError("Email", "Email already registered");
                    return View(user);
                }

                user.PasswordHash = _passwordHasher.HashPassword(user, user.PasswordHash);
                _db.Users.Add(user);
                _db.SaveChanges();

                return RedirectToAction("Login");
            }
            return View(user);
        }

        // GET: /Auth/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Auth/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("", "Please enter username and password");
                return View();
            }

            var user = _db.Users.SingleOrDefault(u => u.Username == username);
            if (user == null)
            {
                ModelState.AddModelError("", "Invalid username or password");
                return View();
            }

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
            if (result == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError("", "Invalid username or password");
                return View();
            }

            // สร้าง Claims สำหรับ Authentication
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email)
            };

            HttpContext.Session.Remove("SessionCart");

            var claimsIdentity = new ClaimsIdentity(claims, "MyCookieAuth");
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            await HttpContext.SignInAsync("MyCookieAuth", claimsPrincipal);

            return RedirectToAction("Index", "Home");
        }

        // POST: /Auth/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Remove("SessionCart");
            await HttpContext.SignOutAsync("MyCookieAuth");
            return RedirectToAction("Index", "Home");
        }
    }
}
