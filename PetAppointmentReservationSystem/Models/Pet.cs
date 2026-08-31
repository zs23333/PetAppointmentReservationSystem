using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetAppointmentReservationSystem.Models
{
    public class Pet
    {
        public int PetId { get; set; }

        [Required]
        public string Name { get; set; }

        public string Species { get; set; }

        public string Breed { get; set; }

        // Required so a pet can't exist without a photo. Note: since the photo itself
        // arrives as an IFormFile (not bound directly to this string), the controller
        // validates file presence manually and only then fills PhotoPath — see
        // PetController.Create for how this attribute is actually enforced.
        [Required(ErrorMessage = "A photo is required to register a pet.")]
        public string PhotoPath { get; set; }

        public int OwnerId { get; set; }

        [ForeignKey(nameof(OwnerId))]
        public User Owner { get; set; }
    }
}