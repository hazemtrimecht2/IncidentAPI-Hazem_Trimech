using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using IncidentAPI_Hazem_Trimech.Data;
using IncidentAPI_Hazem_Trimech.Models;
using Microsoft.EntityFrameworkCore;
using IncidentAPI_Hazem_Trimech.Controllers;

namespace AppTests
{
    [Xunit.Trait("Category","Unit")]
    public class IncidentsTests
    {
        private IncidentsDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<IncidentsDbContext>()
                .UseInMemoryDatabase(System.Guid.NewGuid().ToString())
                .Options;
            return new IncidentsDbContext(options);
        }

        [Fact]
        public async System.Threading.Tasks.Task GetIncidents_WhenDataExists_ReturnsAllIncidents()
        {
            var context = GetDbContext();
            context.Incidents.AddRange(
                new Incident { Title = "Incident1", Status = "OPEN", Severity = "HIGH", Description = "d1" },
                new Incident { Title = "Incident2", Status = "CLOSED", Severity = "LOW", Description = "d2" }
            );
            context.SaveChanges();
            var controller = new IncidentsDbController(context);
            var result = await controller.GetIncidents();
            var incidents = Assert.IsType<List<Incident>>(result.Value);
            Assert.Equal(2, incidents.Count);
        }

        [Fact]
        public async System.Threading.Tasks.Task GetIncident_ExistingId_ReturnsIncident()
        {
            var context = GetDbContext();
            var incident = new Incident { Id = 1, Title = "Test", Status = "OPEN", Severity = "LOW", Description = "d" };
            context.Incidents.Add(incident);
            context.SaveChanges();
            var controller = new IncidentsDbController(context);
            var result = await controller.GetIncident(1);
            Assert.NotNull(result.Value);
            Assert.Equal("Test", result.Value.Title);
        }

        [Fact]
        public async System.Threading.Tasks.Task PostIncident_ValidData_CreatesIncident()
        {
            var context = GetDbContext();
            var controller = new IncidentsDbController(context);
            var incident = new Incident { Title = "NewIncident", Status = "OPEN", Severity = "LOW", Description = "d" };
            var result = await controller.PostIncident(incident);
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var createdIncident = Assert.IsType<Incident>(createdResult.Value);
            Assert.Equal("NewIncident", createdIncident.Title);
            Assert.Equal(1, context.Incidents.Count());
        }

        [Fact]
        public async System.Threading.Tasks.Task PutIncident_ValidUpdate_ReturnsNoContent()
        {
            var context = GetDbContext();
            var incident = new Incident { Id = 1, Title = "Old", Status = "OPEN", Severity = "LOW", Description = "d" };
            context.Incidents.Add(incident);
            context.SaveChanges();
            var controller = new IncidentsDbController(context);
            incident.Title = "Updated";
            var result = await controller.PutIncident(1, incident);
            Assert.IsType<NoContentResult>(result);
            Assert.Equal("Updated", context.Incidents.First().Title);
        }

        [Fact]
        public async System.Threading.Tasks.Task PutIncident_IdMismatch_ReturnsBadRequest()
        {
            var context = GetDbContext();
            var controller = new IncidentsDbController(context);
            var incident = new Incident { Id = 2, Title = "Test", Status = "OPEN", Severity = "LOW", Description = "d" };
            var result = await controller.PutIncident(1, incident);
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async System.Threading.Tasks.Task DeleteIncident_ExistingId_RemovesIncident()
        {
            var context = GetDbContext();
            var incident = new Incident { Id = 1, Title = "DeleteMe", Status = "OPEN", Severity = "LOW", Description = "d" };
            context.Incidents.Add(incident);
            context.SaveChanges();
            var controller = new IncidentsDbController(context);
            var result = await controller.DeleteIncident(1);
            Assert.IsType<NoContentResult>(result);
            Assert.Empty(context.Incidents);
        }

        [Fact]
        public async System.Threading.Tasks.Task DeleteIncident_NotFound_ReturnsNotFound()
        {
            var context = GetDbContext();
            var controller = new IncidentsDbController(context);
            var result = await controller.DeleteIncident(99);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async System.Threading.Tasks.Task FilterBySeverityAsync_ValidSeverity_ReturnsFilteredData()
        {
            var context = GetDbContext();
            context.Incidents.AddRange(
                new Incident { Title = "A", Severity = "HIGH", Status = "OPEN", Description = "d" },
                new Incident { Title = "B", Severity = "LOW", Status = "OPEN", Description = "d" }
            );
            context.SaveChanges();
            var controller = new IncidentsDbController(context);
            var result = await controller.FilterBySeverityAsync("HIGH");
            var okResult = Assert.IsType<OkObjectResult>(result);
            var data = Assert.IsAssignableFrom<List<Incident>>(okResult.Value);
            Assert.Single(data);
        }

        [Fact]
        public async System.Threading.Tasks.Task PostIncident_MissingTitle_ReturnsBadRequest()
        {
            var context = GetDbContext();
            var controller = new IncidentsDbController(context);
            var incident = new Incident
            {
                Status = "OPEN",
                Severity = "HIGH",
                Description = "d"
            };
            controller.ModelState.AddModelError("Title", "Required");
            var result = await controller.PostIncident(incident);
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Theory]
        [InlineData("Low")]
        [InlineData("Medium")]
        [InlineData("High")]
        [InlineData("Critical")]
        public async System.Threading.Tasks.Task PostIncident_ValidSeverity_ReturnsCreated(string severity)
        {
            var context = GetDbContext();
            var controller = new IncidentsDbController(context);
            var incident = new Incident
            {
                Title = "Test",
                Status = "OPEN",
                Severity = severity,
                Description = "d"
            };
            var result = await controller.PostIncident(incident);
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var createdIncident = Assert.IsType<Incident>(createdResult.Value);
            Assert.Equal(severity, createdIncident.Severity);
        }

        [Theory]
        [InlineData("Abc")]
        [InlineData("Azerty")]
        [InlineData("123")]
        [InlineData("")]
        public async System.Threading.Tasks.Task PostIncident_InvalidSeverity_ReturnsBadRequest(string severity)
        {
            var context = GetDbContext();
            var controller = new IncidentsDbController(context);
            var incident = new Incident
            {
                Title = "Test",
                Status = "OPEN",
                Severity = severity,
                Description = "d"
            };
            controller.ModelState.AddModelError("Severity", "Invalid severity");
            var result = await controller.PostIncident(incident);
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }
    }
}
