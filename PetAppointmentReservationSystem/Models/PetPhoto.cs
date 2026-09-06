using System.ComponentModel.DataAnnotations.Schema;

namespace PetAppointmentReservationSystem.Models
{
    public class PetPhoto
    {
        public int PetPhotoId { get; set; }

        public int PetId { get; set; }

        [ForeignKey(nameof(PetId))]
        public Pet Pet { get; set; }

        public string PhotoPath { get; set; }
    }
}