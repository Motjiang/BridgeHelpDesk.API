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

        [Fact]
        public async Task GetAuditTrails_ReturnsNotFound_WhenListIsEmpty()
        {
            // Arrange
            A.CallTo(() => _sender.Send(A<GetAllAuditTrailsQuery>._, A<CancellationToken>._))
                .Returns(Task.FromResult(Enumerable.Empty<API.Models.Domain.AuditTrail>()));

            // Act
            var result = await _controller.GetAuditTrails();

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
            var notFound = result as NotFoundObjectResult;
            notFound!.StatusCode.Should().Be(404);
            notFound.Value.Should().Be("No audit trails found.");
        }

        [Fact]
        public async Task GetAuditTrailById_ReturnsOk_WhenAuditTrailExists()
        {
            // Arrange
            var auditTrail = new API.Models.Domain.AuditTrail
            {
                Id = 1,
                Action = "Create",
                TableAffected = "Tickets",
                Timestamp = DateTime.Now,
                ApplicationUserId = "0001",
                ApplicationUser = new API.Models.Domain.ApplicationUser
                {
                    Id = "0001",
                    UserName = "testuser"
                }
            };

            A.CallTo(() => _sender.Send(A<GetAuditTrailByIdQuery>.That.Matches(q => q.AuditTrailId == 1),
                A<CancellationToken>._))
             .Returns(auditTrail); // Return single object

            // Act
            var result = await _controller.GetAuditTrailById(1);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult!.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(auditTrail); // Deep compare
        }

        [Fact]
        public async Task GetAuditTrailById_ReturnsNotFound_WhenAuditTrailDoesNotExist()
        {
            // Arrange
            A.CallTo(() => _sender.Send(A<GetAuditTrailByIdQuery>.That.Matches(q => q.AuditTrailId == 99), A<CancellationToken>._))
                .Returns(Task.FromResult<API.Models.Domain.AuditTrail>(null));

            // Act
            var result = await _controller.GetAuditTrailById(99);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
            var notFound = result as NotFoundObjectResult;
            notFound!.StatusCode.Should().Be(404);
            notFound.Value.Should().Be("Audit trail with ID 99 not found.");
        }
    }
}
