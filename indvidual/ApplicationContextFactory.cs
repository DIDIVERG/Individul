using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Npgsql;

namespace indvidual;

public class ApplicationContextFactory : IDesignTimeDbContextFactory<ApplicationContext>
{
   
    public NpgsqlConnectionStringBuilder connectionString = new NpgsqlConnectionStringBuilder()
    {

        Host = "localhost",
        Port = 5432,
        Database = "reports_bd",
        Username = "postgres",
        Password = "Arista666",
        IncludeErrorDetail = true
    };

    // implementation of  IDesignTimeDbContextFactory
    public ApplicationContext CreateDbContext(string[] args)
    {
        
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationContext>();
        optionsBuilder.UseNpgsql(connectionString.ConnectionString).UseSnakeCaseNamingConvention();
        return new ApplicationContext(optionsBuilder.Options);
    }
}