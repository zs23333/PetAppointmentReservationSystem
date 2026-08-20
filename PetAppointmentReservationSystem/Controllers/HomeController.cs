using Microsoft.AspNetCore.Mvc;

namespace PetAppointmentReservationSystem.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}