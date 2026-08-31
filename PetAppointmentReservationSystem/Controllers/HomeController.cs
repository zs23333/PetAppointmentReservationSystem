using Microsoft.AspNetCore.Mvc;

namespace PetAppointmentReservationSystem.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.Username = HttpContext.Session.GetString("Username");
            ViewBag.Role = HttpContext.Session.GetString("Role");
            return View();
        }
    }
}