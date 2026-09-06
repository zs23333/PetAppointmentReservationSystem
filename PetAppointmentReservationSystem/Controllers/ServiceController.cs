using Microsoft.AspNetCore.Mvc;
using PetAppointmentReservationSystem.Models;
using System.Collections.Generic;

namespace PetAppointmentReservationSystem.Controllers
{
    public class ServicesController : Controller
    {
        // Static list — matches the 3 services your Book Appointment dropdown offers.
        private static readonly List<ServiceCardVM> AllServices = new()
        {
            new ServiceCardVM
            {
                Name = "Grooming",
                Description = "Pet grooming services including baths, haircuts, and nail trimming.",
                IconClass = "bi-scissors",
                IconColor = "#8f6cff"
            },
            new ServiceCardVM
            {
                Name = "Vaccination",
                Description = "Vaccination services to keep your pets healthy and protected.",
                IconClass = "bi-eyedropper",
                IconColor = "#2dd4bf"
            },
            new ServiceCardVM
            {
                Name = "Checkup",
                Description = "Routine health check-ups and consultations for your pet.",
                IconClass = "bi-heart-pulse",
                IconColor = "#f2a154"
            }
        };

        public IActionResult Index()
        {
            return View(AllServices);
        }
    }
}