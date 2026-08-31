namespace PetAppointmentReservationSystem.Models
{
    // Fixed dropdown list for appointment services — single source of truth
    // so the Create/Edit views and any future validation stay in sync.
    public static class ServiceCatalog
    {
        public static readonly string[] Services = { "Grooming", "Vaccination", "Checkup" };
    }
}