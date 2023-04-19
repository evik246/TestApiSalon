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

            var flagProperties = properties
                .Where(p => p.GetCustomAttributes<NullablePropertyAttribute>().Any())
                .Select(p => p.GetCustomAttribute<NullablePropertyAttribute>()?.FlagPropertyName)
                .Select(f => type.GetProperty(f ?? ""))
                .Where(p => p != null)
                .ToList();


            foreach ( var property in properties )
            {
                var nullableProperty = property.GetCustomAttribute<NullablePropertyAttribute>();

                if (nullableProperty != null)
                {
                    if (property.GetValue(value) != null)
                    {
                        return true;
                    }

                    PropertyInfo? flagProperty = type.GetProperty(nullableProperty.FlagPropertyName)
                        ?? throw new InvalidOperationException
                        ($"Property {nullableProperty.FlagPropertyName} does not exist");

                    if (flagProperty.GetValue(value) is bool isNull && isNull)
                    {
                        return true;
                    }
                }

                if (property.GetValue(value) != null && !flagProperties.Contains(property))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
