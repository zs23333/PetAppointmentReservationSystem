using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetAppointmentReservationSystem.Helpers;
using PetAppointmentReservationSystem.Models;
using System.Linq;

namespace PetAppointmentReservationSystem.Controllers
{
    public class ServicesController : Controller
    {
        private readonly PetConnectContext _context;
        private readonly IWebHostEnvironment _env;

        public ServicesController(PetConnectContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public IActionResult Index()
        {
            var services = _context.Services.ToList();
            return View(services);
        }

        [Authorize(Roles = "Staff")]
        [HttpGet]
        public IActionResult Create()
        {
            return View(new Service());
        }

        [Authorize(Roles = "Staff")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Service model, IFormFile photo)
        {
            ModelState.Remove(nameof(Service.PhotoPath));

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (photo != null && photo.Length > 0)
            {
                if (!PhotoHelper.IsValidPhoto(photo, out var photoError))
                {
                    ModelState.AddModelError(string.Empty, photoError);
                    return View(model);
                }
                model.PhotoPath = PhotoHelper.SavePhoto(photo, _env);
            }

            _context.Services.Add(model);
            _context.SaveChanges();

            TempData["Message"] = $"{model.Name} service added.";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Staff")]
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var service = _context.Services.Find(id);
            if (service == null)
            {
                return NotFound();
            }
            return View(service);
        }

        [Authorize(Roles = "Staff")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Service model, IFormFile photo)
        {
            if (id != model.ServiceId)
            {
                return NotFound();
            }

            var existing = _context.Services.AsNoTracking().FirstOrDefault(s => s.ServiceId == id);
            if (existing == null)
            {
                return NotFound();
            }

            ModelState.Remove(nameof(Service.PhotoPath));
            model.PhotoPath = existing.PhotoPath;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (photo != null && photo.Length > 0)
            {
                if (!PhotoHelper.IsValidPhoto(photo, out var photoError))
                {
                    ModelState.AddModelError(string.Empty, photoError);
                    return View(model);
                }
                model.PhotoPath = PhotoHelper.SavePhoto(photo, _env);
            }

            _context.Services.Update(model);
            _context.SaveChanges();

            TempData["Message"] = $"{model.Name} service updated.";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Staff")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var service = _context.Services.Find(id);
            if (service != null)
            {
                _context.Services.Remove(service);
                _context.SaveChanges();
                TempData["Message"] = "Service deleted.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}