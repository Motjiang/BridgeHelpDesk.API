using BridgeHelpDesk.API.Controllers;
using BridgeHelpDesk.API.Features.Ticket.Commands;
using BridgeHelpDesk.API.Features.Tickets.Commands;
using BridgeHelpDesk.API.Features.Tickets.Queries;
using BridgeHelpDesk.API.Models.Domain;
using BridgeHelpDesk.API.Models.DTOs.Dashboard;
using FakeItEasy;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BridgeHelpDesk.UnitTests.Controllers.Ticket
{
    public class TicketControllerTests
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ISender _mediator;
        private readonly TicketController _controller;

        public TicketControllerTests()
        {
            _userManager = A.Fake<UserManager<ApplicationUser>>();
            _mediator = A.Fake<ISender>();
            _controller = new TicketController(_userManager, _mediator);
        }

        [Fact]
        public async Task LogTicket_Returns_CreatedAtActionResult()
        {
            // Arrange
            var user = new ApplicationUser { Id = "001", UserName = "testuser" };
            var command = new CreateTicketCommand("Network issue", "Cannot access internet", "Information Technology");

            A.CallTo(() => _userManager.GetUserAsync(A<ClaimsPrincipal>._)).Returns(user);
            A.CallTo(() => _mediator.Send(command, A<CancellationToken>._)).Returns(1);

            // Act
            var result = await _controller.LogTicket(command);

            // Assert
            result.Should().BeOfType<CreatedAtActionResult>();
            var createdResult = result as CreatedAtActionResult;
            createdResult!.RouteValues["id"].Should().Be(1);
        }

        [Fact]
        public async Task GetResolvedTickets_ReturnsOk_WithListOfTickets()
        {
            // Arrange
            var tickets = new List<API.Models.Domain.Ticket>
            {
                new API.Models.Domain.Ticket { Id = 1, Title = "Ticket 1" },
                new API.Models.Domain.Ticket { Id = 2, Title = "Ticket 2" }
            };

            A.CallTo(() => _mediator.Send(A<GetResolovedByTicketsQuery>._, default)).Returns(tickets);

            // Act
            var result = await _controller.GetResolvedTickets();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult!.Value.Should().BeEquivalentTo(tickets);
        }

        [Fact]
        public async Task GetResolvedTickets_ReturnsNotFound_WhenNoTicketsFound()
        {
            // Arrange
            var tickets = new List<API.Models.Domain.Ticket>(); 
            A.CallTo(() => _mediator.Send(A<GetResolovedByTicketsQuery>._, default))
                .Returns(Task.FromResult(tickets)); 

            // Act
            var result = await _controller.GetResolvedTickets();

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
            var notFoundResult = result as NotFoundObjectResult;
            notFoundResult!.Value.Should().Be("No resolved tickets found.");
        }

        [Fact]
        public async Task GetTickets_ReturnsOk_WhenTicketsAreFound()
        {
            // Arrange
            var tickets = new List<API.Models.Domain.Ticket>
            {
                new API.Models.Domain.Ticket { Id = 1, Title = "Login issue" },
                new API.Models.Domain.Ticket { Id = 2, Title = "Printer not working" }
            };

            A.CallTo(() => _mediator.Send(A<GetAllTicketsQuery>._, default))
                .Returns(Task.FromResult<IEnumerable<API.Models.Domain.Ticket>>(tickets));

            // Act
            var result = await _controller.GetTickets();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult!.Value.Should().BeEquivalentTo(tickets);
        }

        [Fact]
        public async Task GetTickets_ReturnsNotFound_WhenNoTicketsExist()
        {
            // Arrange
            var emptyTickets = new List<API.Models.Domain.Ticket>(); // could also be null

            A.CallTo(() => _mediator.Send(A<GetAllTicketsQuery>._, default))
                .Returns(Task.FromResult<IEnumerable<API.Models.Domain.Ticket>>(emptyTickets));

            // Act
            var result = await _controller.GetTickets();

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
            var notFoundResult = result as NotFoundObjectResult;
            notFoundResult!.Value.Should().Be("No tickets found.");
        }

        [Fact]
        public async Task GetTicketById_ReturnsOk_WhenTicketExists()
        {
            // Arrange
            int ticketId = 1;
            var ticket = new API.Models.Domain.Ticket { Id = ticketId, Title = "Slow internet" };

            A.CallTo(() => _mediator.Send(
                A<GetTicketByIdQuery>.That.Matches(q => q.TicketId == ticketId), default))
                .Returns(Task.FromResult(ticket));

            // Act
            var result = await _controller.GetTicketById(ticketId);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult!.Value.Should().BeEquivalentTo(ticket);
        }

        [Fact]
        public async Task GetTicketById_ReturnsNotFound_WhenTicketDoesNotExist()
        {
            // Arrange
            int ticketId = 99;

            A.CallTo(() => _mediator.Send(
                A<GetTicketByIdQuery>.That.Matches(q => q.TicketId == ticketId), default))
                .Returns(Task.FromResult<API.Models.Domain.Ticket>(null));

            // Act
            var result = await _controller.GetTicketById(ticketId);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
            var notFound = result as NotFoundObjectResult;
            notFound!.Value.Should().Be("Ticket with ID 99 not found.");
        }

        [Fact]
        public async Task UpdateTicket_ReturnsOk_WhenTicketIsUpdated()
        {
            // Arrange
            int ticketId = 1;
            var command = new UpdateTicketCommand(ticketId, "Updated Title", "Updated description","Information Technology");
            
            A.CallTo(() => _mediator.Send(command, default)).Returns(true);

            // Act
            var result = await _controller.UpdateTicket(ticketId, command);

            // Assert
            result.Should().BeOfType<OkResult>();
        }

        [Fact]
        public async Task UpdateTicket_ReturnsBadRequest_WhenCommandIsNull()
        {
            // Arrange
            int ticketId = 1;

            // Act
            var result = await _controller.UpdateTicket(ticketId, null);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequest = result as BadRequestObjectResult;
            badRequest!.Value.Should().Be("Invalid ticket data.");
        }

        [Fact]
        public async Task UpdateTicket_ReturnsBadRequest_WhenTicketIdMismatch()
        {
            // Arrange
            int ticketId = 2;
            var command = new UpdateTicketCommand(ticketId, "Updated Title", "Updated description", "Information Technology");

            // Act
            var result = await _controller.UpdateTicket(1, command); 

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequest = result as BadRequestObjectResult;
            badRequest!.Value.Should().Be("Invalid ticket data.");
        }

        [Fact]
        public async Task UpdateTicket_ReturnsNotFound_WhenUpdateFails()
        {
            // Arrange
            int ticketId = 5;
            var command = new UpdateTicketCommand(ticketId, "Updated Title", "Updated description", "Information Technology");
            
            A.CallTo(() => _mediator.Send(command, default)).Returns(false); // simulate failure

            // Act
            var result = await _controller.UpdateTicket(ticketId, command);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
            var notFound = result as NotFoundObjectResult;
            notFound!.Value.Should().Be($"Ticket with ID {ticketId} not found or could not be updated.");
        }

        [Fact]
        public async Task DeleteTicket_ReturnsNoContent_WhenDeletionIsSuccessful()
        {
            // Arrange
            int ticketId = 1;

            A.CallTo(() => _mediator.Send(A<DeleteTicketCommand>.That.Matches(c => c.ticketId == ticketId), default))
                .Returns(true);

            // Act
            var result = await _controller.DeleteTicket(ticketId);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task GetDashboardSummary_ReturnsOk_WhenSummaryExists()
        {
            // Arrange
            var summary = new TechnicianDashboardDto
            {
                TotalTicketsResolved = 5,
                TotalTicketsLogged = 13,
                TotalTicketsDeleted = 8
            };

            A.CallTo(() => _mediator.Send(A<TechnicianDashboardQuery>._, default))
                .Returns(Task.FromResult(summary));

            // Act
            var result = await _controller.GetDashboardSummary();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult!.Value.Should().BeEquivalentTo(summary);
        }

        [Fact]
        public async Task GetDashboardSummary_ReturnsNotFound_WhenSummaryIsNull()
        {
            // Arrange
            A.CallTo(() => _mediator.Send(A<TechnicianDashboardQuery>._, default))
                .Returns(Task.FromResult<TechnicianDashboardDto>(null));

            // Act
            var result = await _controller.GetDashboardSummary();

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
            var notFound = result as NotFoundObjectResult;
            notFound!.Value.Should().Be("Dashboard summary not found.");
        }
    }
}
