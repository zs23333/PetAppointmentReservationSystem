using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PetAppointmentReservationSystem.Models;
using System.Linq;
using System.Security.Claims;

namespace PetAppointmentReservationSystem.Controllers
{
    [Authorize(Roles = "Customer")]
    public class AppointmentController : Controller
    {
        private readonly PetConnectContext _context;

        public AppointmentController(PetConnectContext context)
        {
            _context = context;
        }

        private int CurrentUserId =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        // Customer's own "Appointment Book" — only their pets' appointments.
        public IActionResult List()
        {
            var appointments = _context.Appointments
                .Include(a => a.Staff)
                .Include(a => a.Pet)
                .Where(a => a.Pet.OwnerId == CurrentUserId)
                .OrderBy(a => a.Date)
                .ToList();

            return View(appointments);
        }

        [HttpGet]
        public IActionResult Create()
        {
            PopulateDropdowns();
            return View(new Appointment { Date = DateTime.Today.AddHours(9), DurationMinutes = 30 });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Appointment model, string newPetName, string newPetSpecies, string newPetBreed)
        {
            // Auto-create a pet if the customer didn't pick one from the dropdown
            // but typed a new pet's name instead.
            if (model.PetId == 0 && !string.IsNullOrWhiteSpace(newPetName))
            {
                var newPet = new Pet
                {
                    Name = newPetName,
                    Species = newPetSpecies,
                    Breed = newPetBreed,
                    OwnerId = CurrentUserId,
                    PhotoPath = "/images/pets/placeholder.png" // no upload here; can be added later via Pet/Edit
                };
                _context.Pets.Add(newPet);
                _context.SaveChanges();
                model.PetId = newPet.PetId;
                ModelState.Remove(nameof(model.PetId));
            }

            if (model.PetId == 0)
            {
                ModelState.AddModelError(string.Empty, "Please select an existing pet or enter a new pet's name.");
            }

            if (!ModelState.IsValid)
            {
                PopulateDropdowns();
                return View(model);
            }

            _context.Appointments.Add(model);
            _context.SaveChanges();

            TempData["Message"] = "Appointment booked successfully.";
            return RedirectToAction(nameof(List));
        }

        private void PopulateDropdowns(int? selectedPetId = null, int? selectedStaffId = null)
        {
            var myPets = _context.Pets.Where(p => p.OwnerId == CurrentUserId).ToList();
            ViewBag.PetList = new SelectList(myPets, "PetId", "Name", selectedPetId);

            // Always queried fresh from the database, so newly registered staff
            // appear immediately without any manual seeding step.
            var staff = _context.StaffMembers.OrderBy(s => s.Name).ToList();
            ViewBag.StaffList = new SelectList(staff, "StaffId", "Name", selectedStaffId);

            ViewBag.ServiceList = new SelectList(ServiceCatalog.Services);
        }
    }
}