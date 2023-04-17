using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace TestApiSalon.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class AtLeastOnePropertyAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value == null) { return true; }

            var type = value.GetType();
            var properties = type.GetProperties();
            foreach ( var property in properties )
            {
                var exclude = property.GetCustomAttribute<ExcludeFromValidationAttribute>();

                if (property.GetValue(value) != null && exclude == null)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
