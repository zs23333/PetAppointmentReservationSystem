using Microsoft.AspNetCore.Mvc;
using PetAppointmentReservationSystem.Models;

namespace PetAppointmentReservationSystem.Controllers
{
    public class AppointmentController : Controller
    {
        [HttpGet]
        public IActionResult List()
        {
            var today = DateTime.Today;

            var timeSlots = new List<DateTime>();
            for (var t = today.AddHours(8); t <= today.AddHours(18); t = t.AddMinutes(30))
            {
                timeSlots.Add(t);
            }

            var staffColumns = new List<StaffColumnViewModel>
            {
                new StaffColumnViewModel
                {
                    StaffName = "Dr. Alice Tan",
                    Appointments = new List<AppointmentViewModel>
                    {
                        new AppointmentViewModel
                        {
                            Id = 1,
                            PetName = "Toby",
                            OwnerName = "James Lee",
                            ServiceName = "Grooming",
                            ServiceType = "Grooming",
                            DurationMinutes = 60,
                            StartTime = today.AddHours(9),
                            StaffName = "Dr. Alice Tan",
                            Notes = "Full wash, trim, and nail clip.",
                            Phone = "012-3456789",
                            Email = "james.lee@example.com"
                        },
                        new AppointmentViewModel
                        {
                            Id = 2,
                            PetName = "Rex",
                            OwnerName = "Mei Ling",
                            ServiceName = "Vaccination",
                            ServiceType = "Medical",
                            DurationMinutes = 30,
                            StartTime = today.AddHours(11),
                            StaffName = "Dr. Alice Tan",
                            Notes = "Annual rabies booster.",
                            Phone = "012-9988776",
                            Email = "mei.ling@example.com"
                        }
                    }
                },
                new StaffColumnViewModel
                {
                    StaffName = "Dr. Ben Wong",
                    Appointments = new List<AppointmentViewModel>
                    {
                        new AppointmentViewModel
                        {
                            Id = 3,
                            PetName = "Doggo",
                            OwnerName = "Sarah Lim",
                            ServiceName = "General Checkup",
                            ServiceType = "Medical",
                            DurationMinutes = 45,
                            StartTime = today.AddHours(10),
                            StaffName = "Dr. Ben Wong",
                            Notes = "Follow-up on skin allergy.",
                            Phone = "013-2223344",
                            Email = "sarah.lim@example.com"
                        }
                    }
                },
                new StaffColumnViewModel
                {
                    StaffName = "Groomer Nisha",
                    Appointments = new List<AppointmentViewModel>
                    {
                        new AppointmentViewModel
                        {
                            Id = 4,
                            PetName = "Rex",
                            OwnerName = "Mei Ling",
                            ServiceName = "Nail Trim",
                            ServiceType = "Grooming",
                            DurationMinutes = 20,
                            StartTime = today.AddHours(14),
                            StaffName = "Groomer Nisha",
                            Notes = "Quick trim, pet is anxious with clippers.",
                            Phone = "012-9988776",
                            Email = "mei.ling@example.com"
                        }
                    }
                }
            };

            var model = new CalendarViewModel
            {
                Date = today,
                StaffColumns = staffColumns,
                TimeSlots = timeSlots
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult Book()
        {
            var model = new AppointmentVM
            {
                Date = DateTime.Today.AddHours(9),
                DurationMinutes = 30
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Book(AppointmentVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            TempData["Message"] = $"Appointment booked for {model.PetName} with {model.StaffName}.";
            return RedirectToAction(nameof(List));
        }
    }
}