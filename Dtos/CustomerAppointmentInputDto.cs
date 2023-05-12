using System.ComponentModel.DataAnnotations;
using TestApiSalon.Attributes;

namespace TestApiSalon.Dtos
{
    public class CustomerAppointmentInputDto
    {
        [Required(ErrorMessage = "Master id is required")]
        public required int MasterId { get; set; }

        [Required(ErrorMessage = "Service id is required")]
        public required int ServiceId { get; set; }

        [Required(ErrorMessage = "Date is required")]
        [NotPast(ErrorMessage = "Date must not be in the past")]
        public required DateOnly Date { get; set; }
    }
}
