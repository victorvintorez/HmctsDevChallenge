using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace HmctsDevChallenge.Backend.Database;

public class HmctsDbContextFactory : IDesignTimeDbContextFactory<HmctsDbContext>
{
    public HmctsDbContext CreateDbContext(string[] args)
    {
        // Build configuration from appsettings.json
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", false, false)
            .AddJsonFile("appsettings.Development.json", true, false)
            .AddEnvironmentVariables()
            .Build();

        // Get connection string from configuration
        var connectionString = configuration.GetConnectionString("Database");

        if (string.IsNullOrEmpty(connectionString))
            throw new InvalidOperationException(
                "Connection string 'Database' not found in appsettings.json");

        // Build DbContextOptions
        var optionsBuilder = new DbContextOptionsBuilder<HmctsDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new HmctsDbContext(optionsBuilder.Options);
    }
}