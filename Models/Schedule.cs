using System.ComponentModel;
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
        [Description("Monday")]
        Monday = 0,
        [Description("Tuesday")]
        Tuesday = 1,
        [Description("Wednesday")]
        Wednesday = 2,
        [Description("Thursday")]
        Thursday = 3,
        [Description("Friday")]
        Friday = 4,
        [Description("Saturday")]
        Saturday = 5,
        [Description("Sunday")]
        Sunday = 6
    }
}
