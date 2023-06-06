using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using TestApiSalon.Models;

namespace TestApiSalon.Dtos.Schedule
{
    public class MasterScheduleCreateDto
    {
        [Required(ErrorMessage = "Day of the week is required")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Weekday Weekday { get; set; }

        [Required(ErrorMessage = "Start time of the work day is required")]
        public TimeOnly StartTime { get; set; }

        [Required(ErrorMessage = "End time of the work day is required")]
        public TimeOnly EndTime { get; set; }
    }
}
