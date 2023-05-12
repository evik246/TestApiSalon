using System.ComponentModel.DataAnnotations;
using TestApiSalon.Attributes;

namespace TestApiSalon.Dtos.Appointment
{
    public class AppointmentCreateDto
    {
        [Required(ErrorMessage = "Date is required")]
        [NotPast(ErrorMessage = "Date must not be in the past")]
        public DateTimeOffset Date { get; set; }

        [Required(ErrorMessage = "Customer id is required")]
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Service id is required")]
        public int ServiceId { get; set; }

        [Required(ErrorMessage = "Employee id is required")]
        public int EmployeeId { get; set; }
    }
}
