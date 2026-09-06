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
        public IActionResult Register() => View(new RegisterVM());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterVM model)
        {
            if (!ModelState.IsValid) return View(model);

            if (_context.Users.Any(u => u.Username == model.Username))
            {
                ModelState.AddModelError(nameof(model.Username), "That username is already taken.");
                return View(model);
            }

            var isStaff = model.Role == "Staff";

            var user = new User
            {
                FullName = model.Name,
                Username = model.Username,
                Password = model.Password,
                Email = model.Email,
                Role = model.Role,
                IsApproved = !isStaff // Staff starts unapproved; Customer is approved immediately.
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            // Note: the Staff table row (and dropdown visibility) is only created
            // once an Admin approves — see AdminController.Approve.

            TempData["Message"] = isStaff
                ? "Account created. Please wait for approval from Admin before you can log in."
                : $"Account created for {model.Name}. Please log in.";

            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public IActionResult Login() => View(new LoginVM());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = _context.Users.FirstOrDefault(u =>
                u.Username == model.Username && u.Password == model.Password);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid username or password.");
                return View(model);
            }

            if (user.Role == "Staff" && !user.IsApproved)
            {
                ModelState.AddModelError(string.Empty,
                    "Please wait for approval from Admin before you can log in.");
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