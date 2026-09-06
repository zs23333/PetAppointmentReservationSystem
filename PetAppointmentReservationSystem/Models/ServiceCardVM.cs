namespace PetAppointmentReservationSystem.Models
{
    // Static display model only — NOT database-backed. Services here are just
    // fixed content for the Services page; Appointment.Service stays a plain string.
    public class ServiceCardVM
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string IconClass { get; set; } // Bootstrap Icons class
        public string IconColor { get; set; }
    }
}