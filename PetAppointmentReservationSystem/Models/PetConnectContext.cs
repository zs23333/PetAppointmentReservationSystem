using Microsoft.EntityFrameworkCore;

namespace PetAppointmentReservationSystem.Models
{
    public class PetConnectContext : DbContext
    {
        public PetConnectContext(DbContextOptions<PetConnectContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Staff> StaffMembers { get; set; }
        public DbSet<Pet> Pets { get; set; }
        public DbSet<Appointment> Appointments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Optional starter data — new staff registered through the app get
            // inserted automatically by AccountController.Register, so this seed
            // is just so the dropdown isn't empty on a fresh database.
            modelBuilder.Entity<Staff>().HasData(
                new Staff { StaffId = 1, Name = "Dr. Alice Tan", Role = "Veterinarian" },
                new Staff { StaffId = 2, Name = "Dr. Ben Wong", Role = "Veterinarian" },
                new Staff { StaffId = 3, Name = "Nisha", Role = "Groomer" }
            );
        }
    }
}