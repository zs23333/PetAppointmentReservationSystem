namespace PetAppointmentReservationSystem.Models
{
    public class StaffColumnViewModel
    {
        public string StaffName { get; set; }
        public List<AppointmentViewModel> Appointments { get; set; } = new();
    }
}