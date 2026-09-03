using System.ComponentModel.DataAnnotations;

namespace PetAppointmentReservationSystem.Models
{
    public class User
    {
        public int UserId { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Username { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; }

        [Required]
        public string Role { get; set; } // "Customer" or "Staff"

        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string Email { get; set; }
    }
}