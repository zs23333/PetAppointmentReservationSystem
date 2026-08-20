using Microsoft.AspNetCore.Mvc;

namespace PetAppointmentReservationSystem.Controllers
{
    public class InfoController : Controller
    {
        public IActionResult ComingSoon(string section)
        {
            ViewData["Section"] = string.IsNullOrWhiteSpace(section) ? "This page" : section;
            return View();
        }
    }
}