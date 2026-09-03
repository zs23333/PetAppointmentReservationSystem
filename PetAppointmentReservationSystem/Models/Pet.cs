using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetAppointmentReservationSystem.Models
{
    public class Pet
    {
        public int PetId { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(50)]
        public string Species { get; set; }

        [StringLength(50)]
        public string Breed { get; set; }

        [Required(ErrorMessage = "A photo is required to register a pet.")]
        public string PhotoPath { get; set; }

        public int OwnerId { get; set; }

        [ForeignKey(nameof(OwnerId))]
        public User Owner { get; set; }
    }
}