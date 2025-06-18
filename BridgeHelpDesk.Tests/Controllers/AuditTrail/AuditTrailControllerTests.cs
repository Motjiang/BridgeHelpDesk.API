using BridgeHelpDesk.API.Controllers;
using BridgeHelpDesk.API.Features.AuditTrail.Queries;
using FakeItEasy;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BridgeHelpDesk.UnitTests.Controllers.AuditTrail
{
    public class AuditTrailControllerTests
    {
        private readonly AuditTrailController _controller;
        private readonly ISender _sender;

        public AuditTrailControllerTests()
        {
            _sender = A.Fake<ISender>();
            _controller = new AuditTrailController(_sender);
        }

        [Fact]
        public async Task GetAuditTrails_ReturnsOk_WithListOfAuditTrails()
        {
            // Arrange
            var auditTrails = new List<API.Models.Domain.AuditTrail>
            {
                new API.Models.Domain.AuditTrail
                {
                    Id = 1,
                    Action = "Create",
                    TableAffected = "Ticktes",
                    Timestamp = DateTime.Now,
                    ApplicationUserId = "0001",
                    ApplicationUser = new API.Models.Domain.ApplicationUser { Id = "0001", UserName = "testuser" }
                },
                new API.Models.Domain.AuditTrail
                {
                    Id = 2,
                    Action = "Update",
                    TableAffected = "Tickets",
                    Timestamp = DateTime.Now.AddMinutes(-5),
                    ApplicationUserId = "0002",
                    ApplicationUser = new API.Models.Domain.ApplicationUser { Id = "0002", UserName = "anotheruser" }
                }
            };

            A.CallTo(() => _sender.Send(A<GetAllAuditTrailsQuery>._, A<CancellationToken>._))
                .Returns(Task.FromResult(auditTrails.AsEnumerable()));

            // Act
            var result = await _controller.GetAuditTrails();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult!.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(auditTrails);
        }
    }
}
