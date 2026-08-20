namespace PetAppointmentReservationSystem.Models
{
    public class AppointmentViewModel
    {
        public int Id { get; set; }
        public string PetName { get; set; }
        public string OwnerName { get; set; }
        public string ServiceName { get; set; }
        public string ServiceType { get; set; }
        public int DurationMinutes { get; set; }
        public DateTime StartTime { get; set; }
        public string StaffName { get; set; }
        public string Notes { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
    }
}