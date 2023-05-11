using System.ComponentModel.DataAnnotations;

namespace TestApiSalon.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class BirthdayAttribute : ValidationAttribute
    {
        public int MinAge { get; set; }
        public int MaxAge { get; set; }

        public override bool IsValid(object? value)
        {
            if (value == null) { return true; }

            if (value is DateTime date)
            {
                if (date.AddYears(MinAge) > DateTime.Now)
                {
                    return false;
                }

                return date.AddYears(MaxAge) > DateTime.Now;
            }

            if (value is DateOnly dateOnly)
            {
                DateTime dateTemp = dateOnly.ToDateTime(new TimeOnly());
                if (dateTemp.AddYears(MinAge) > DateTime.Now)
                {
                    return false;
                }

                return dateTemp.AddYears(MaxAge) > DateTime.Now;
            }
            return false;
        }
    }
}
