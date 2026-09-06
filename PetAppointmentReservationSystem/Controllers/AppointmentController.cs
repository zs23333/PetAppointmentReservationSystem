using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PetAppointmentReservationSystem.Helpers;
using PetAppointmentReservationSystem.Models;
using System.Linq;
using System.Security.Claims;

namespace PetAppointmentReservationSystem.Controllers
{
    [Authorize(Roles = "Customer")]
    public class AppointmentController : Controller
    {
        private readonly PetConnectContext _context;
        private readonly IWebHostEnvironment _env;

        public AppointmentController(PetConnectContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        private int CurrentUserId =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        public IActionResult List()
        {
            var appointments = GetMyAppointments();
            return View(appointments);
        }

        // AJAX endpoint: returns just the table rows, filtered by search term.
        [HttpGet]
        public IActionResult Search(string q)
        {
            var appointments = GetMyAppointments(q);
            return PartialView("_AppointmentRows", appointments);
        }

        private List<Appointment> GetMyAppointments(string q = null)
        {
            var query = _context.Appointments
                .Include(a => a.Staff)
                .Include(a => a.Pet)
                .Where(a => a.Pet.OwnerId == CurrentUserId);

            if (!string.IsNullOrWhiteSpace(q))
            {
                query = query.Where(a =>
                    a.Pet.Name.Contains(q) ||
                    a.Service.Contains(q) ||
                    (a.Staff != null && a.Staff.Name.Contains(q)));
            }

            return query.OrderBy(a => a.Date).ToList();
        }

        [HttpGet]
        public IActionResult Create(string service)
        {
            PopulateDropdowns();
            var model = new Appointment
            {
                Date = DateTime.Today.AddHours(9),
                DurationMinutes = 30,
                Service = service // prefills the dropdown/field if passed from a Services card click
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(
            Appointment model,
            string newPetName,
            string newPetSpecies,
            string newPetBreed,
            IFormFile newPetPhoto)
        {
            var creatingNewPet = model.PetId == 0;

            if (creatingNewPet)
            {
                // Inline pet registration requires name + a valid photo.
                ModelState.Remove(nameof(model.PetId));

                if (string.IsNullOrWhiteSpace(newPetName))
                {
                    ModelState.AddModelError(string.Empty, "Enter a name for the new pet, or select an existing one.");
                }

                if (!PhotoHelper.IsValidPhoto(newPetPhoto, out var photoError))
                {
                    ModelState.AddModelError(string.Empty, photoError);
                }
            }
            else
            {
                // Confirm the selected pet actually belongs to this customer.
                var ownsPet = _context.Pets.Any(p => p.PetId == model.PetId && p.OwnerId == CurrentUserId);
                if (!ownsPet)
                {
                    ModelState.AddModelError(string.Empty, "Please select one of your registered pets.");
                }
            }

            if (!ModelState.IsValid)
            {
                PopulateDropdowns(model.PetId, model.StaffId);
                return View(model);
            }

            if (creatingNewPet)
            {
                var newPet = new Pet
                {
                    Name = newPetName,
                    Species = newPetSpecies,
                    Breed = newPetBreed,
                    OwnerId = CurrentUserId,
                    PhotoPath = PhotoHelper.SavePhoto(newPetPhoto, _env)
                };
                _context.Pets.Add(newPet);
                _context.SaveChanges();
                model.PetId = newPet.PetId;
            }

            _context.Appointments.Add(model);
            _context.SaveChanges();

            var owner = _context.Users.Find(CurrentUserId);
            var petName = _context.Pets.Find(model.PetId)?.Name ?? "your pet";
            var emailSent = EmailHelper.SendAppointmentConfirmation(owner?.Email, petName, model.Service, model.Date);

            TempData["Message"] = emailSent
                ? $"Appointment booked for {petName}. A confirmation email was sent to {owner?.Email}."
                : $"Appointment booked for {petName}.";

            return RedirectToAction(nameof(List));
        }

        private void PopulateDropdowns(int? selectedPetId = null, int? selectedStaffId = null)
        {
            var myPets = _context.Pets.Where(p => p.OwnerId == CurrentUserId).ToList();
            ViewBag.PetList = new SelectList(myPets, "PetId", "Name", selectedPetId);

            var staff = _context.StaffMembers.OrderBy(s => s.Name).ToList();
            ViewBag.StaffList = new SelectList(staff, "StaffId", "Name", selectedStaffId);

            var services = new List<string> { "Grooming", "Vaccination", "Checkup" };
ViewBag.ServiceList = new SelectList(services);
        }
    }
}