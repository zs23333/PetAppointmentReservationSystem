using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetAppointmentReservationSystem.Models;
using System.Linq;

namespace PetAppointmentReservationSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly PetConnectContext _context;

        public AdminController(PetConnectContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var pending = _context.Users
                .Where(u => u.Role == "Staff" && !u.IsApproved)
                .OrderBy(u => u.FullName)
                .ToList();

            return View(pending);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Approve(int id)
        {
            var user = _context.Users.Find(id);
            if (user != null && user.Role == "Staff")
            {
                user.IsApproved = true;
                _context.SaveChanges();

                // Creates the Staff row now — this is what makes them appear
                // in the appointment-booking dropdown, with zero manual seeding.
                var alreadyStaff = _context.StaffMembers.Any(s => s.UserId == user.UserId);
                if (!alreadyStaff)
                {
                    _context.StaffMembers.Add(new Staff
                    {
                        Name = user.FullName,
                        Role = "Staff",
                        UserId = user.UserId
                    });
                    _context.SaveChanges();
                }

                TempData["Message"] = $"{user.FullName} has been approved.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Reject(int id)
        {
            var user = _context.Users.Find(id);
            if (user != null && user.Role == "Staff" && !user.IsApproved)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
                TempData["Message"] = "Staff application rejected and removed.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}