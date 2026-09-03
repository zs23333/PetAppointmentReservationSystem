using System.ComponentModel.DataAnnotations;

namespace PetAppointmentReservationSystem.Models
{
    public class Staff
    {
        public int StaffId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(50)]
        public string Role { get; set; }

        public int? UserId { get; set; }

        public ICollection<Appointment> Appointments { get; set; }
    }
}