using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.Linq;
using IncidentAPI_Hazem_Trimech.Data;

namespace AppTests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");
            builder.ConfigureServices((context, services) =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<IncidentsDbContext>));
                if (descriptor != null) services.Remove(descriptor);

                // Read connection string from configuration so pipeline can override it via env var
                var configuration = context.Configuration;
                var connectionString = configuration.GetConnectionString("IncidentsConnection");

                if (string.IsNullOrWhiteSpace(connectionString))
                {
                    // Local dev / CI fallback: use InMemory when no connection string supplied
                    services.AddDbContext<IncidentsDbContext>(options =>
                        options.UseInMemoryDatabase("Incidents_Test_Db"));
                }
                else
                {
                    services.AddDbContext<IncidentsDbContext>(options =>
                        options.UseSqlServer(connectionString));
                }

                var sp = services.BuildServiceProvider();
                using (var scope = sp.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<IncidentsDbContext>();
                    db.Database.EnsureDeleted();
                    db.Database.EnsureCreated();
                }
            });
        }
    }
}
