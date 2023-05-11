using System.Text.Json.Serialization;

namespace TestApiSalon.Models
{
    public class Schedule
    {
        public int Id { get; set; }

        public Weekday Weekday { get; set; }

        public TimeOnly StartTime { get; set; }

        public TimeOnly EndTime { get; set; }

        [JsonIgnore]
        public int EmployeeId { get; set; }
    }

    public enum Weekday
    {
        Monday, 
        Tuesday, 
        Wednesday,
        Thursday,
        Friday,
        Saturday,
        Sunday
    }
}
