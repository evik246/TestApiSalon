using System.ComponentModel.DataAnnotations;
using TestApiSalon.Attributes;

namespace TestApiSalon.Dtos.Appointment
{
    public class AppointmentCreateDto
    {
        [Required(ErrorMessage = "Date is required")]
        [NotPast(ErrorMessage = "Date must not be in the past")]
        public required DateTimeOffset Date { get; set; }

        [Required(ErrorMessage = "Service id is required")]
        public required int ServiceId { get; set; }

        [Required(ErrorMessage = "Employee id is required")]
        public required int EmployeeId { get; set; }
    }
}
