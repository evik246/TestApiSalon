using System.Text.Json.Serialization;

namespace TestApiSalon.Models
{
    public class Schedule
    {
        public int Id { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Weekday Weekday { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        [JsonIgnore]
        public int EmployeeId { get; set; }
    }

    public enum Weekday
    {
        Monday = 0, 
        Tuesday = 1, 
        Wednesday = 2,
        Thursday = 3,
        Friday = 4,
        Saturday = 5,
        Sunday = 6
    }
}
