using System.ComponentModel.DataAnnotations;

namespace PetAppointmentReservationSystem.Models
{
    public class AppointmentVM
    {
        [Required]
        [Display(Name = "Pet Name")]
        public string PetName { get; set; }

        [Required]
        [Display(Name = "Owner Name")]
        public string OwnerName { get; set; }

        [Required]
        [Display(Name = "Service")]
        public string ServiceName { get; set; }

        public int ServiceId { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Display(Name = "Duration (minutes)")]
        [Range(15, 480)]
        public int DurationMinutes { get; set; }

        [Display(Name = "Staff Member")]
        public string StaffName { get; set; }
    }
}