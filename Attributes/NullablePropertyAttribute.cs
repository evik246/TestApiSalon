namespace TestApiSalon.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class NullablePropertyAttribute : Attribute
    {
        public string FlagPropertyName { get; set; }

        public NullablePropertyAttribute(string flagPropertyName)
        {
            FlagPropertyName = flagPropertyName;
        }
    }
}
