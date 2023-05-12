using System.ComponentModel.DataAnnotations;

namespace TestApiSalon.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class NotPastAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value == null) return true;

            if (value is DateTime date)
            {
                return date >= DateTime.Now;
            }

            if (value is DateOnly dateOnly)
            {
                return dateOnly.ToDateTime(TimeOnly.MinValue) >= DateTime.Today;
            }
            return false;
        }
    }
}
