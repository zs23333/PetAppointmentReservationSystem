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

        private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        public IActionResult Index()
        {
            var pets = _context.Pets
                .Include(p => p.Owner)
                .Where(p => p.OwnerId == CurrentUserId)
                .ToList();
            return View(pets);
        }

        public IActionResult Details(int id)
        {
            var pet = _context.Pets.Include(p => p.Photos)
                .FirstOrDefault(p => p.PetId == id && p.OwnerId == CurrentUserId);
            if (pet == null) return NotFound();
            return View(pet);
        }

        [HttpGet]
        public IActionResult Create() => View(new Pet());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Pet model, List<IFormFile> photos)
        {
            ModelState.Remove(nameof(Pet.PhotoPath));
            ModelState.Remove(nameof(Pet.OwnerId));
            ModelState.Remove(nameof(Pet.Owner));

            var validPhotos = photos?.Where(p => p != null && p.Length > 0).ToList() ?? new();

            if (validPhotos.Count == 0)
            {
                ModelState.AddModelError(string.Empty, "At least one photo is required.");
            }
            else
            {
                foreach (var p in validPhotos)
                {
                    if (!PhotoHelper.IsValidPhoto(p, out var err))
                    {
                        ModelState.AddModelError(string.Empty, err);
                        break;
                    }
                }
            }

            if (!ModelState.IsValid) return View(model);

            model.OwnerId = CurrentUserId;
            model.PhotoPath = PhotoHelper.SavePhoto(validPhotos[0], _env);

            _context.Pets.Add(model);
            _context.SaveChanges();

            foreach (var extra in validPhotos.Skip(1))
            {
                _context.PetPhotos.Add(new PetPhoto
                {
                    PetId = model.PetId,
                    PhotoPath = PhotoHelper.SavePhoto(extra, _env)
                });
            }
            _context.SaveChanges();

            TempData["Message"] = $"{model.Name} has been added with {validPhotos.Count} photo(s).";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var pet = _context.Pets.FirstOrDefault(p => p.PetId == id && p.OwnerId == CurrentUserId);
            if (pet == null) return NotFound();
            return View(pet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Pet model, List<IFormFile> photos)
        {
            if (id != model.PetId) return NotFound();

            var existing = _context.Pets.AsNoTracking()
                .FirstOrDefault(p => p.PetId == id && p.OwnerId == CurrentUserId);
            if (existing == null) return NotFound();

            ModelState.Remove(nameof(Pet.PhotoPath));
            ModelState.Remove(nameof(Pet.OwnerId));
            ModelState.Remove(nameof(Pet.Owner));

            model.PhotoPath = existing.PhotoPath;
            model.OwnerId = existing.OwnerId;

            var validPhotos = photos?.Where(p => p != null && p.Length > 0).ToList() ?? new();

            foreach (var p in validPhotos)
            {
                if (!PhotoHelper.IsValidPhoto(p, out var err))
                {
                    ModelState.AddModelError(string.Empty, err);
                    break;
                }
            }

            if (!ModelState.IsValid) return View(model);

            if (validPhotos.Count > 0)
            {
                model.PhotoPath = PhotoHelper.SavePhoto(validPhotos[0], _env);
            }

            _context.Pets.Update(model);
            _context.SaveChanges();

            foreach (var extra in validPhotos.Skip(1))
            {
                _context.PetPhotos.Add(new PetPhoto
                {
                    PetId = model.PetId,
                    PhotoPath = PhotoHelper.SavePhoto(extra, _env)
                });
            }
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