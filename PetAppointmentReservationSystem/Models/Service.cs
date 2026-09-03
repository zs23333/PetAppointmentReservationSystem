using System.ComponentModel.DataAnnotations;

namespace PetAppointmentReservationSystem.Models
{
    public class Service
    {
        public int ServiceId { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(300)]
        public string Description { get; set; }

        [Range(0, 100000)]
        public decimal Price { get; set; }

        [Display(Name = "Duration (minutes)")]
        [Range(5, 480)]
        public int DurationMinutes { get; set; }

        public string PhotoPath { get; set; }
    }
}