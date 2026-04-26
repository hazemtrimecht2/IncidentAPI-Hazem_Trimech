using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IncidentAPI_Hazem_Trimech.Data;
using IncidentAPI_Hazem_Trimech.Models;

namespace IncidentAPI_Hazem_Trimech.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IncidentsDbController : ControllerBase
    {
        private readonly IncidentsDbContext _context;

        public IncidentsDbController(IncidentsDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<Incident>>> GetIncidents()
        {
            return await _context.Incidents.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Incident>> GetIncident(int id)
        {
            var incident = await _context.Incidents.FindAsync(id);
            if (incident == null) return NotFound();
            return incident;
        }

        [HttpPost]
        public async Task<ActionResult<Incident>> PostIncident(Incident incident)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.Incidents.Add(incident);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetIncident), new { id = incident.Id }, incident);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutIncident(int id, Incident incident)
        {
            if (id != incident.Id) return BadRequest();
            _context.Entry(incident).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteIncident(int id)
        {
            var incident = await _context.Incidents.FindAsync(id);
            if (incident == null) return NotFound();
            _context.Incidents.Remove(incident);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("filter-by-severity-async")]
        public async Task<IActionResult> FilterBySeverityAsync([FromQuery] string severity)
        {
            var data = await _context.Incidents
                .Where(i => i.Severity.Contains(severity, StringComparison.OrdinalIgnoreCase))
                .ToListAsync();
            return Ok(data);
        }
    }
}
