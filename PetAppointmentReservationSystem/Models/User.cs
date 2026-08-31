using System.ComponentModel.DataAnnotations;

namespace PetAppointmentReservationSystem.Models
{
    public class User
    {
        public int UserId { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Role { get; set; } // "Customer" or "Staff"

        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}