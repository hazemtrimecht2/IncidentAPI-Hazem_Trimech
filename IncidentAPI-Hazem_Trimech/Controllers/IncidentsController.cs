using IncidentAPI_Hazem_Trimech.Models;//permet d'utiliser la class incident 
using Microsoft.AspNetCore.Mvc;//contient controllerBase  IActionReukt httpgeto post 

namespace IncidentAPI_Hazem_Trimech.Controllers
{
    [ApiController] //tkhalina njario les eureur autmatique du model par exemple kel faza fel enumeration  et simplifier les reponse htttp 
    [Route("api/[controller]")]//definit le reoute principale 
    public class IncidentsController : ControllerBase
    {
        // 🔹 Stockage en mémoire
        private static readonly List<Incident> _incidents = new();//declaration d'ue liste statique qui contient les incident 
        private static int _nextId = 1;//id autoicremente

        private static readonly string[] AllowedSeverities =
        { "LOW", "MEDIUM", "HIGH", "CRITICAL" };

        private static readonly string[] AllowedStatuses =
        { "OPEN", "IN_PROGRESS", "RESOLVED" };

        [HttpPost("create-incident")]//c'est un route de type post 
        public IActionResult CreateIncident([FromBody] Incident incident)//recoit un objet dans frombody 
        {
            if (!AllowedSeverities.Contains(incident.Severity))
                return BadRequest("Severity invalide.");

            incident.Id = _nextId++;
            incident.CreatedAt = DateTime.Now;
            incident.Status = "OPEN"; // accepted from work branch

            _incidents.Add(incident);

            return Ok(incident);
        }



        [HttpGet("get-all")]
        public IActionResult GetAllIncidents()
        {
            return Ok(_incidents);
        }



        [HttpGet("getbyid/{id}")]
        public IActionResult GetIncidentById(int id)
        {
            var incident = _incidents.FirstOrDefault(i => i.Id == id);

            if (incident == null)
                return NotFound();

            return Ok(incident);
        }


        [HttpPut("update-status/{id}")]
        public IActionResult UpdateIncidentStatus(int id, [FromBody] string status)
        {
            var incident = _incidents.FirstOrDefault(i => i.Id == id);

            if (incident == null)
                return NotFound();

            if (!AllowedStatuses.Contains(status))
                return BadRequest("Status invalide.");

            incident.Status = status;

            return Ok(incident);
        }



        [HttpDelete("delete-incident/{id}")]
        public IActionResult DeleteIncident(int id)
        {
            var incident = _incidents.FirstOrDefault(i => i.Id == id);

            if (incident == null)
                return NotFound();


            if (incident.Severity == "CRITICAL" && incident.Status == "OPEN")
                return BadRequest("Impossible de supprimer un incident CRITICAL encore OPEN.");

            _incidents.Remove(incident);

            return NoContent();
        }



        [HttpGet("filter-by-status")]
        public IActionResult FilterByStatus([FromQuery] string status)
        {
            if (string.IsNullOrWhiteSpace(status))
                return BadRequest("Le paramètre status est obligatoire.");

            var filteredIncidents = _incidents
                .Where(i => i.Status.Contains(status, StringComparison.OrdinalIgnoreCase))
                .ToList();

            return Ok(filteredIncidents);
        }


        [HttpGet("filter-by-severity")]
        public IActionResult FilterBySeverity([FromQuery] string severity)
        {
            if (string.IsNullOrWhiteSpace(severity))
                return BadRequest("Le paramètre severity est obligatoire.");

            var filteredIncidents = _incidents
                .Where(i => i.Severity.Contains(severity, StringComparison.OrdinalIgnoreCase))
                .ToList();

            return Ok(filteredIncidents);
        }

        [HttpGet("filter-by-status-async")]
        public Task<IActionResult> FilterByStatusAsync([FromQuery] string status)
        {
            if (string.IsNullOrWhiteSpace(status))
                return Task.FromResult<IActionResult>(BadRequest("Le paramètre status est obligatoire."));

            var filteredIncidents = _incidents
                .Where(i => i.Status.Contains(status, StringComparison.OrdinalIgnoreCase))
                .ToList();

            return Task.FromResult<IActionResult>(Ok(filteredIncidents));
        }

        [HttpGet("filter-by-severity-async")]
        public Task<IActionResult> FilterBySeverityAsync([FromQuery] string severity)
        {
            if (string.IsNullOrWhiteSpace(severity))
                return Task.FromResult<IActionResult>(BadRequest("Le paramètre severity est obligatoire."));

            var filteredIncidents = _incidents
                .Where(i => i.Severity.Contains(severity, StringComparison.OrdinalIgnoreCase))
                .ToList();

            return Task.FromResult<IActionResult>(Ok(filteredIncidents));
        }

        // Action ajoutée par mon collaborateur
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> PutIncidentStatus(int id, string status)
        {
            if (!AllowedStatuses.Contains(status.ToUpper()))
            {
                return BadRequest($"Status must be one of the following: {string.Join(", ", AllowedStatuses)}");
            }
            var incident = _incidents.FirstOrDefault(i => i.Id == id);
            if (incident == null)
                return NotFound();
            incident.Status = status;
            // In-memory list: no DbContext SaveChangesAsync needed, but keep await for signature
            await Task.CompletedTask;
            return NoContent();
        }

        // Test des pull requests !!!


    }
}