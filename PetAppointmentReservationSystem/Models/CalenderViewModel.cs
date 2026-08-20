namespace PetAppointmentReservationSystem.Models
{
    public class CalendarViewModel
    {
        public DateTime Date { get; set; }
        public List<StaffColumnViewModel> StaffColumns { get; set; } = new();
        public List<DateTime> TimeSlots { get; set; } = new();
    }
}