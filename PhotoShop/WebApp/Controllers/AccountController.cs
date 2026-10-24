using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebApp.Data;
using WebApp.Models;
using WebApp.ViewModels;

namespace WebApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;
        public AccountController(AppDbContext context) { _context = context; }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var user = await _context.AppUser.FirstOrDefaultAsync(u => u.Username == model.Username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
            {
                ModelState.AddModelError("", "Neispravno korisničko ime ili lozinka.");
                return View(model);
            }
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.Username),
                new(ClaimTypes.Role, user.Role),
                new(ClaimTypes.Email, user.Email)
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
            return user.Role == "Admin" ? RedirectToAction("Index", "Photo") : RedirectToAction("Browse", "Photo");
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            if (await _context.AppUser.AnyAsync(u => u.Username == model.Username || u.Email == model.Email))
            {
                ModelState.AddModelError("", "Korisnik s tim korisničkim imenom ili emailom već postoji.");
                return View(model);
            }
            var user = new AppUser
            {
                Name = model.Username, Username = model.Username, Email = model.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                FirstName = model.FirstName, LastName = model.LastName, Role = "User"
            };
            _context.AppUser.Add(user);
            await _context.SaveChangesAsync();
            return RedirectToAction("Login");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var user = await _context.AppUser.FindAsync(userId);
            if (user == null) return NotFound();
            return View(new ProfileViewModel
            {
                Username = user.Username, Email = user.Email,
                FirstName = user.FirstName, LastName = user.LastName, Phone = user.Phone
            });
        }

        [HttpPost]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<IActionResult> UpdateProfile([FromBody] ProfileViewModel model)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var user = await _context.AppUser.FindAsync(userId);
            if (user == null) return Json(new { success = false, message = "Korisnik nije pronađen." });
            user.Email = model.Email; user.FirstName = model.FirstName;
            user.LastName = model.LastName; user.Phone = model.Phone;
            if (!string.IsNullOrEmpty(model.NewPassword))
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Profil uspješno ažuriran." });
        }
    }
}
