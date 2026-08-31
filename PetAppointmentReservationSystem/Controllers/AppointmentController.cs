using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PetAppointmentReservationSystem.Helpers;
using PetAppointmentReservationSystem.Models;
using System.Linq;

namespace PetAppointmentReservationSystem.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly PetConnectContext _context;

        public AppointmentController(PetConnectContext context)
        {
            _context = context;
        }

        public IActionResult List()
        {
            var appointments = _context.Appointments
                .Include(a => a.Staff)
                .OrderBy(a => a.Date)
                .ToList();

            return View(appointments);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.StaffList = new SelectList(_context.StaffMembers.ToList(), "StaffId", "Name");
            return View(new Appointment { Date = DateTime.Today.AddHours(9), DurationMinutes = 30 });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Appointment model, string ownerEmail)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.StaffList = new SelectList(_context.StaffMembers.ToList(), "StaffId", "Name", model.StaffId);
                return View(model);
            }

            _context.Appointments.Add(model);
            _context.SaveChanges();

            var emailSent = EmailHelper.SendAppointmentConfirmation(ownerEmail, model.PetName, model.Service, model.Date);

            TempData["Message"] = emailSent
                ? $"Appointment booked for {model.PetName}. A confirmation email was sent to {ownerEmail}."
                : $"Appointment booked for {model.PetName}.";

            return RedirectToAction(nameof(List));
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var appointment = _context.Appointments.Find(id);
            if (appointment == null)
            {
                return NotFound();
            }

            ViewBag.StaffList = new SelectList(_context.StaffMembers.ToList(), "StaffId", "Name", appointment.StaffId);
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
                ViewBag.StaffList = new SelectList(_context.StaffMembers.ToList(), "StaffId", "Name", model.StaffId);
                return View(model);
            }

            _context.Appointments.Update(model);
            _context.SaveChanges();

            TempData["Message"] = $"Appointment for {model.PetName} was updated.";
            return RedirectToAction(nameof(List));
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

            return RedirectToAction(nameof(List));
        }
    }
}