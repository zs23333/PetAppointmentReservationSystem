using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetAppointmentReservationSystem.Models
{
    public class Appointment
    {
        public int AppointmentId { get; set; }

        [Required]
        [Display(Name = "Pet Name")]
        public string PetName { get; set; }

        [Required]
        [Display(Name = "Owner Name")]
        public string OwnerName { get; set; }

        [Display(Name = "Staff Member")]
        public int StaffId { get; set; }

        [ForeignKey(nameof(StaffId))]
        public Staff Staff { get; set; }

        [Required]
        public string Service { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Display(Name = "Duration (minutes)")]
        [Range(15, 480)]
        public int DurationMinutes { get; set; }
    }
}