using Microsoft.EntityFrameworkCore;
using IncidentAPI_Hazem_Trimech.Models;

namespace IncidentAPI_Hazem_Trimech.Data
{
    public class IncidentsDbContext : DbContext
    {
        public IncidentsDbContext(DbContextOptions<IncidentsDbContext> options) : base(options) { }

        public DbSet<Incident> Incidents { get; set; }
    }
}
