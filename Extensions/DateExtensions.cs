namespace TestApiSalon.Extensions
{
    public static class DateExtensions
    {
        public static DateTime ToCorrectDateTime(this DateTime date) 
        {
            TimeZoneInfo tz = TimeZoneInfo.Local;
            return TimeZoneInfo.ConvertTime(date, tz);
        }

        public static DateTime ToCorrectDateTime(this DateTimeOffset date)
        {
            return date.DateTime;
        }
    }
}
