using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PetAppointmentReservationSystem.Models;
using System.Linq;

namespace PetAppointmentReservationSystem.Controllers
{
    [Authorize(Roles = "Staff")]
    public class ManageAppointmentsController : Controller
    {
        private readonly PetConnectContext _context;

        public ManageAppointmentsController(PetConnectContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var appointments = GetAllAppointments();
            return View(appointments);
        }

        // AJAX endpoint: filtered rows for staff search/filter box.
        [HttpGet]
        public IActionResult Search(string q)
        {
            var appointments = GetAllAppointments(q);
            return PartialView("_ManageAppointmentRows", appointments);
        }

        private List<Appointment> GetAllAppointments(string q = null)
        {
            var query = _context.Appointments
                .Include(a => a.Staff)
                .Include(a => a.Pet)
                    .ThenInclude(p => p.Owner)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                query = query.Where(a =>
                    a.Pet.Name.Contains(q) ||
                    a.Service.Contains(q) ||
                    (a.Staff != null && a.Staff.Name.Contains(q)) ||
                    (a.Pet.Owner != null && a.Pet.Owner.Username.Contains(q)));
            }

            return query.OrderBy(a => a.Date).ToList();
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var appointment = _context.Appointments.Find(id);
            if (appointment == null)
            {
                return NotFound();
            }

            PopulateDropdowns(appointment.PetId, appointment.StaffId);
            return View(appointment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Appointment model)
        {
            if (id != model.AppointmentId)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                PopulateDropdowns(model.PetId, model.StaffId);
                return View(model);
            }

            _context.Appointments.Update(model);
            _context.SaveChanges();

            TempData["Message"] = "Appointment updated.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var appointment = _context.Appointments.Find(id);
            if (appointment != null)
            {
                _context.Appointments.Remove(appointment);
                _context.SaveChanges();
                TempData["Message"] = "Appointment deleted.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult PetDetails(int id)
        {
            var pet = _context.Pets
                .Include(p => p.Photos)
                .Include(p => p.Owner)
                .FirstOrDefault(p => p.PetId == id);

            if (pet == null) return NotFound();

            return View(pet); // can reuse Pet/Details.cshtml's markup, or a staff-specific copy
        }

        private void PopulateDropdowns(int? selectedPetId = null, int? selectedStaffId = null)
        {
            var pets = _context.Pets.Include(p => p.Owner).ToList();
            ViewBag.PetList = new SelectList(pets, "PetId", "Name", selectedPetId);

            var staff = _context.StaffMembers.OrderBy(s => s.Name).ToList();
            ViewBag.StaffList = new SelectList(staff, "StaffId", "Name", selectedStaffId);

            var services = new List<string> { "Grooming", "Vaccination", "Checkup" };
            ViewBag.ServiceList = new SelectList(services);
        }
    }
}