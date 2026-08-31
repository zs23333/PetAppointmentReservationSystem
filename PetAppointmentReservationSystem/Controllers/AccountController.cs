using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PetAppointmentReservationSystem.Models;
using System.Linq;

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
                Password = model.Password, // NOTE: hash this in a real deployment
                Email = model.Email,
                Role = model.Role
            };

            _context.Users.Add(user);
            _context.SaveChanges();

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
        public IActionResult Login(LoginVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = _context.Users.FirstOrDefault(u =>
                (u.Username == model.Username || u.Email == model.Email) &&
                u.Password == model.Password);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid username/email or password.");
                return View(model);
            }

            HttpContext.Session.SetString("Username", user.Username);
            HttpContext.Session.SetString("Role", user.Role);
            HttpContext.Session.SetInt32("UserId", user.UserId);

            TempData["Message"] = $"Welcome back, {user.Username}!";
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["Message"] = "You have been logged out.";
            return RedirectToAction("Index", "Home");
        }
    }
}