namespace TestApiSalon.Models
{
    public class User
    {
        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public UserRole Role { get; set; }
    }

    public enum UserRole
    {
        Client = 0,
        Master = 1,
        Manager = 2,
        Admin = 3
    }
}
