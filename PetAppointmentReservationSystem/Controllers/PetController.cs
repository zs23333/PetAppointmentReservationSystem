using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetAppointmentReservationSystem.Helpers;
using PetAppointmentReservationSystem.Models;
using System.Linq;
using System.Security.Claims;

namespace PetAppointmentReservationSystem.Controllers
{
    [Authorize(Roles = "Customer")]
    public class PetController : Controller
    {
        private readonly PetConnectContext _context;
        private readonly IWebHostEnvironment _env;

        public PetController(PetConnectContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        private int CurrentUserId =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        public IActionResult Index()
        {
            var pets = _context.Pets
                .Where(p => p.OwnerId == CurrentUserId)
                .Include(p => p.Owner)
                .ToList();
            return View(pets);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new Pet());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Pet model, IFormFile photo)
        {
            ModelState.Remove(nameof(Pet.PhotoPath));
            ModelState.Remove(nameof(Pet.OwnerId));
            ModelState.Remove(nameof(Pet.Owner));

            if (!PhotoHelper.IsValidPhoto(photo, out var photoError))
            {
                ModelState.AddModelError(string.Empty, photoError);
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            model.OwnerId = CurrentUserId; // links to the logged-in AppUser
            model.PhotoPath = PhotoHelper.SavePhoto(photo, _env);

            _context.Pets.Add(model);
            _context.SaveChanges();

            TempData["Message"] = $"{model.Name} has been added.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var pet = _context.Pets.FirstOrDefault(p => p.PetId == id && p.OwnerId == CurrentUserId);
            if (pet == null)
            {
                return NotFound();
            }

            return View(pet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Pet model, IFormFile photo)
        {
            if (id != model.PetId)
            {
                return NotFound();
            }

            var existing = _context.Pets.AsNoTracking()
                .FirstOrDefault(p => p.PetId == id && p.OwnerId == CurrentUserId);
            if (existing == null)
            {
                return NotFound();
            }

            ModelState.Remove(nameof(Pet.PhotoPath));
            ModelState.Remove(nameof(Pet.OwnerId));
            ModelState.Remove(nameof(Pet.Owner));

            model.PhotoPath = existing.PhotoPath;
            model.OwnerId = existing.OwnerId;

            if (photo != null && photo.Length > 0)
            {
                if (!PhotoHelper.IsValidPhoto(photo, out var photoError))
                {
                    ModelState.AddModelError(string.Empty, photoError);
                }
                else
                {
                    model.PhotoPath = PhotoHelper.SavePhoto(photo, _env);
                }
            }
            else if (string.IsNullOrEmpty(existing.PhotoPath))
            {
                ModelState.AddModelError(string.Empty, "A photo is required.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            _context.Pets.Update(model);
            _context.SaveChanges();

            TempData["Message"] = $"{model.Name} has been updated.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var pet = _context.Pets.FirstOrDefault(p => p.PetId == id && p.OwnerId == CurrentUserId);
            if (pet != null)
            {
                _context.Pets.Remove(pet);
                _context.SaveChanges();
                TempData["Message"] = "Pet deleted.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}