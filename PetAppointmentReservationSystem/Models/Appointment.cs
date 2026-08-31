using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetAppointmentReservationSystem.Models
{
    public class Appointment
    {
        public int AppointmentId { get; set; }

        [Required]
        [Display(Name = "Pet")]
        public int PetId { get; set; }

        [ForeignKey(nameof(PetId))]
        public Pet Pet { get; set; }

        [Display(Name = "Staff Member")]
        public int StaffId { get; set; }

        [ForeignKey(nameof(StaffId))]
        public Staff Staff { get; set; }

        [Required]
        [Display(Name = "Service")]
        public string Service { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Display(Name = "Duration (minutes)")]
        [Range(15, 480)]
        public int DurationMinutes { get; set; }
    }
}