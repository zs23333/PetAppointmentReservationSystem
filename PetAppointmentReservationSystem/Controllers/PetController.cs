using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetAppointmentReservationSystem.Models;
using System.IO;
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
            var pets = _context.Pets.Where(p => p.OwnerId == CurrentUserId).ToList();
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
            // PhotoPath isn't submitted directly (it's derived from the uploaded file),
            // so remove it from binding validation and check the actual file instead —
            // this is what makes the [Required] on Pet.PhotoPath meaningfully enforced.
            ModelState.Remove(nameof(Pet.PhotoPath));

            if (photo == null || photo.Length == 0)
            {
                ModelState.AddModelError(string.Empty, "A photo is required to register a pet.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            model.OwnerId = CurrentUserId;
            model.PhotoPath = SavePhoto(photo);

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

            // Keep [Required] meaningful on Edit too: only demand a NEW photo if
            // there's no existing photo to fall back on.
            ModelState.Remove(nameof(Pet.PhotoPath));
            model.PhotoPath = existing.PhotoPath;
            model.OwnerId = existing.OwnerId;

            if (photo == null || photo.Length == 0)
            {
                if (string.IsNullOrEmpty(existing.PhotoPath))
                {
                    ModelState.AddModelError(string.Empty, "A photo is required.");
                }
            }
            else
            {
                model.PhotoPath = SavePhoto(photo);
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

        private string SavePhoto(IFormFile photo)
        {
            var fileName = Guid.NewGuid() + Path.GetExtension(photo.FileName);
            var uploadsFolder = Path.Combine(_env.WebRootPath, "images", "pets");
            Directory.CreateDirectory(uploadsFolder);
            var filePath = Path.Combine(uploadsFolder, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            photo.CopyTo(stream);

            return "/images/pets/" + fileName;
        }
    }
}