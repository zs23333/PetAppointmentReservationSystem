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

        public string PhotoPath { get; set; }

        public int OwnerId { get; set; }

        [ForeignKey(nameof(OwnerId))]
        public User Owner { get; set; }
    }
}