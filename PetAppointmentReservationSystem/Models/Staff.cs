using System.ComponentModel.DataAnnotations;

namespace PetAppointmentReservationSystem.Models
{
    public class Staff
    {
        public int StaffId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Role { get; set; } // e.g. "Staff", "Veterinarian", "Groomer"

        // Links back to the login account that created this staff row (nullable —
        // seeded/legacy staff rows won't have a matching User).
        public int? UserId { get; set; }

        public ICollection<Appointment> Appointments { get; set; }
    }
}