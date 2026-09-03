using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetAppointmentReservationSystem.Models;
using System.Linq;

namespace PetAppointmentReservationSystem.Controllers
{
    [Authorize(Roles = "Staff")]
    public class ReportsController : Controller
    {
        private readonly PetConnectContext _context;

        public ReportsController(PetConnectContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var appointments = _context.Appointments.ToList();

            var byMonth = appointments
                .GroupBy(a => new { a.Date.Year, a.Date.Month })
                .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                .Select(g => new
                {
                    Label = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMM yyyy"),
                    Count = g.Count()
                })
                .ToList();

            var byService = appointments
                .GroupBy(a => a.Service)
                .OrderByDescending(g => g.Count())
                .Select(g => new { Label = g.Key, Count = g.Count() })
                .ToList();

            var vm = new ReportsVM
            {
                MonthLabels = byMonth.Select(m => m.Label).ToList(),
                MonthCounts = byMonth.Select(m => m.Count).ToList(),
                ServiceLabels = byService.Select(s => s.Label).ToList(),
                ServiceCounts = byService.Select(s => s.Count).ToList(),
                TotalAppointments = appointments.Count,
                TotalPets = _context.Pets.Count(),
                TotalCustomers = _context.Users.Count(u => u.Role == "Customer")
            };

            return View(vm);
        }
    }
}