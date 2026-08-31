using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetAppointmentReservationSystem.Models;
using System.IO;
using System.Linq;

namespace PetAppointmentReservationSystem.Controllers
{
    public class PetController : Controller
    {
        private readonly PetConnectContext _context;
        private readonly IWebHostEnvironment _env;

        public PetController(PetConnectContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public IActionResult Index()
        {
            var pets = _context.Pets.Include(p => p.Owner).ToList();
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
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = HttpContext.Session.GetInt32("UserId");
            model.OwnerId = userId ?? 0;

            if (photo != null && photo.Length > 0)
            {
                model.PhotoPath = SavePhoto(photo);
            }

            _context.Pets.Add(model);
            _context.SaveChanges();

            TempData["Message"] = $"{model.Name} has been added.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var pet = _context.Pets.Find(id);
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

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var existing = _context.Pets.AsNoTracking().FirstOrDefault(p => p.PetId == id);
            model.PhotoPath = existing?.PhotoPath;
            model.OwnerId = existing?.OwnerId ?? model.OwnerId;

            if (photo != null && photo.Length > 0)
            {
                model.PhotoPath = SavePhoto(photo);
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
            var pet = _context.Pets.Find(id);
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