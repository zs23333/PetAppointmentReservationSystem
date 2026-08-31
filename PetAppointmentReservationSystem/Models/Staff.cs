using System.ComponentModel.DataAnnotations;

namespace PetAppointmentReservationSystem.Models
{
    public class Staff
    {
        public int StaffId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Role { get; set; } // e.g. "Veterinarian", "Groomer"

        public ICollection<Appointment> Appointments { get; set; }
    }
}