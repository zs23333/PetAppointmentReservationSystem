namespace PetAppointmentReservationSystem.Models
{
    public class ReportsVM
    {
        public List<string> MonthLabels { get; set; } = new();
        public List<int> MonthCounts { get; set; } = new();

        public List<string> ServiceLabels { get; set; } = new();
        public List<int> ServiceCounts { get; set; } = new();

        public int TotalAppointments { get; set; }
        public int TotalPets { get; set; }
        public int TotalCustomers { get; set; }
    }
}