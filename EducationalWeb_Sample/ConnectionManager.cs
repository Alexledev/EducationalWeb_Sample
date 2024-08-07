using Infrastructure.DataAccessLayer;

namespace EducationalWeb_Sample
{
    public static class ConnectionManager
    {
        public static void AddMySQLConnection(this IServiceCollection services)
        {
            IConfiguration configuration = services.BuildServiceProvider().GetService<IConfiguration>();

            ConnectionStringManager.ConnectionString = configuration.GetConnectionString("DefaultConnection")!;
        }
    }
}
