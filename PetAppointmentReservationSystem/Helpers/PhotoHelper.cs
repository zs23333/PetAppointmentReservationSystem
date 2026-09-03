using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Linq;

namespace PetAppointmentReservationSystem.Helpers
{
    public static class PhotoHelper
    {
        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png" };
        private static readonly string[] AllowedContentTypes = { "image/jpeg", "image/png" };
        private const long MaxSizeBytes = 5 * 1024 * 1024; // 5 MB

        public static bool IsValidPhoto(IFormFile file, out string errorMessage)
        {
            errorMessage = null;

            if (file == null || file.Length == 0)
            {
                errorMessage = "A photo is required.";
                return false;
            }

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            var contentType = file.ContentType?.ToLowerInvariant();

            if (!AllowedExtensions.Contains(ext) || !AllowedContentTypes.Contains(contentType))
            {
                errorMessage = "Only JPG and PNG photos are allowed.";
                return false;
            }

            if (file.Length > MaxSizeBytes)
            {
                errorMessage = "Photo must be smaller than 5 MB.";
                return false;
            }

            return true;
        }

        public static string SavePhoto(IFormFile file, IWebHostEnvironment env)
        {
            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var uploadsFolder = Path.Combine(env.WebRootPath, "images", "pets");
            Directory.CreateDirectory(uploadsFolder);
            var filePath = Path.Combine(uploadsFolder, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            file.CopyTo(stream);

            return "/images/pets/" + fileName;
        }
    }
}