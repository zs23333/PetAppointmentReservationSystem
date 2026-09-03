using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using PetAppointmentReservationSystem.Models;
using System.Linq;
using System.Security.Claims;

namespace PetAppointmentReservationSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly PetConnectContext _context;

        public AccountController(PetConnectContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterVM());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (_context.Users.Any(u => u.Username == model.Username))
            {
                ModelState.AddModelError(nameof(model.Username), "That username is already taken.");
                return View(model);
            }

            var user = new User
            {
                Username = model.Username,
                Password = model.Password,
                Email = model.Email,
                Role = model.Role
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            // Staff auto-insert: new staff appear in the appointment dropdown immediately,
            // no manual seeding required.
            if (model.Role == "Staff")
            {
                var alreadyStaff = _context.StaffMembers.Any(s => s.UserId == user.UserId);
                if (!alreadyStaff)
                {
                    _context.StaffMembers.Add(new Staff
                    {
                        Name = model.Name,
                        Role = "Staff",
                        UserId = user.UserId
                    });
                    _context.SaveChanges();
                }
            }

            TempData["Message"] = $"Account created for {model.Name}. Please log in.";
            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginVM());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = _context.Users.FirstOrDefault(u =>
                u.Username == model.Username && u.Password == model.Password);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid username or password.");
                return View(model);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity),
                new AuthenticationProperties { IsPersistent = model.RememberMe });

            TempData["Message"] = $"Welcome back, {user.Username}!";
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            TempData["Message"] = "You have been logged out.";
            return RedirectToAction("Index", "Home");
        }
    }
}