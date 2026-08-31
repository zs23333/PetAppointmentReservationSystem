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

        // Consolidated staff view — every appointment, across every customer.
        public IActionResult Index()
        {
            var appointments = _context.Appointments
                .Include(a => a.Staff)
                .Include(a => a.Pet)
                    .ThenInclude(p => p.Owner)
                .OrderBy(a => a.Date)
                .ToList();

            return View(appointments);
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
        public IActionResult Cancel(int id)
        {
            var appointment = _context.Appointments.Find(id);
            if (appointment != null)
            {
                _context.Appointments.Remove(appointment);
                _context.SaveChanges();
                TempData["Message"] = "Appointment cancelled.";
            }

            return RedirectToAction(nameof(Index));
        }

        private void PopulateDropdowns(int? selectedPetId = null, int? selectedStaffId = null)
        {
            var pets = _context.Pets.Include(p => p.Owner).ToList();
            ViewBag.PetList = new SelectList(pets, "PetId", "Name", selectedPetId);

            var staff = _context.StaffMembers.OrderBy(s => s.Name).ToList();
            ViewBag.StaffList = new SelectList(staff, "StaffId", "Name", selectedStaffId);

            ViewBag.ServiceList = new SelectList(ServiceCatalog.Services);
        }
    }
}