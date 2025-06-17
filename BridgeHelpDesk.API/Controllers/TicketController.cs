using BridgeHelpDesk.API.Features.Ticket.Commands;
using BridgeHelpDesk.API.Features.Tickets.Queries;
using BridgeHelpDesk.API.Models.Domain;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BridgeHelpDesk.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ISender _mediator;

        public TicketController(UserManager<ApplicationUser> userManager, ISender mediator)
        {
            _userManager = userManager;
            _mediator = mediator;
        }

        [HttpPost("log-ticket")]
        public async Task<IActionResult> LogTicket([FromBody] CreateTicketCommand command)
        {
            if (command == null)
                return BadRequest("Invalid ticket data.");

            // Get the currently logged-in user
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized("User not found.");


            // Send the updated command to MediatR
            var ticketId = await _mediator.Send(command);

            return CreatedAtAction(nameof(LogTicket), new { id = ticketId }, null);
        }

        [HttpGet("get-tickets")]
        public async Task<IActionResult> GetTickets()
        {
            var tickets = await _mediator.Send(new GetAllTicketsQuery());

            if (tickets == null || !tickets.Any())
                return NotFound("No tickets found.");

            return Ok(tickets);
        }
    }
}
