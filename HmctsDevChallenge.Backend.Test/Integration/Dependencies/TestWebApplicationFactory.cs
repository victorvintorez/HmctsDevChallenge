using HmctsDevChallenge.Backend.Database;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace HmctsDevChallenge.Backend.Test.Integration.Dependencies;

public class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");

        // Replace the production database with an in-memory database for testing
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<HmctsDbContext>();

            services.AddDbContext<HmctsDbContext>(options => { options.UseInMemoryDatabase("InMemoryTestDb"); });
        });

        builder.ConfigureServices(services =>
        {
            var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<HmctsDbContext>();

            db.Database.EnsureCreated();
        });
    }
}