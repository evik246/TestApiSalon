namespace TestApiSalon.Data
{
    public class DbConnectionManager : IDbConnectionManager
    {
        public DbConnectionName ConnectionName { get; set; }

        public DbConnectionManager() 
        {
            ConnectionName = DbConnectionName.Guest;
        }
    }
}
