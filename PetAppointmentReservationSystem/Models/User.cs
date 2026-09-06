using System.ComponentModel.DataAnnotations;

namespace PetAppointmentReservationSystem.Models
{
    public class User
    {
        public int UserId { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Username { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; }

        [Required]
        public string Role { get; set; } // "Customer", "Staff", "Admin"

        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string Email { get; set; }

        // Customers/Admins are approved by default; Staff must wait for Admin approval.
        public bool IsApproved { get; set; } = true;
    }
}