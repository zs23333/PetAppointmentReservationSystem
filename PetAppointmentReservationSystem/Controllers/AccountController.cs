using Microsoft.AspNetCore.Mvc;
using PetAppointmentReservationSystem.Models;

namespace PetAppointmentReservationSystem.Controllers
{
    public class AccountController : Controller
    {
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

            TempData["Message"] = $"Welcome back, {model.Username ?? model.Email}!";
            return RedirectToAction("Index", "Home");
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

            TempData["Message"] = $"Account created for {model.Name}. Please log in.";
            return RedirectToAction(nameof(Login));
        }
    }
}