using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using TestApiSalon.Attributes;
using TestApiSalon.Models;

namespace TestApiSalon.Dtos.Schedule
{
    [AtLeastOneProperty(ErrorMessage = "At least one property must be specified")]
    public class MasterScheduleChangeDto
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Weekday? Weekday { get; set; }

        public TimeOnly? StartTime { get; set; } = null;

        public TimeOnly? EndTime { get; set; } = null;
    }
}
