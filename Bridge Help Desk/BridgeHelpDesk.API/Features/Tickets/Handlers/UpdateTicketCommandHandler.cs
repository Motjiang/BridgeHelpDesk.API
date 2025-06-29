using BridgeHelpDesk.API.Data;
using BridgeHelpDesk.API.Features.Tickets.Commands;
using BridgeHelpDesk.API.Hubs.Ticket;
using BridgeHelpDesk.API.Models.Domain;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace BridgeHelpDesk.API.Features.Tickets.Handlers
{
    public class UpdateTicketCommandHandler : IRequestHandler<UpdateTicketCommand, bool>
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<TicketHub> _hubContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UpdateTicketCommandHandler(
            ApplicationDbContext context,
            IHubContext<TicketHub> hubContext,
            UserManager<ApplicationUser> userManager,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _hubContext = hubContext;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> Handle(UpdateTicketCommand request, CancellationToken cancellationToken)
        {
            var ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.Id == request.TicketId, cancellationToken);

            var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);

            if (ticket == null || user == null)
            {
                return false; // Ticket or user not found
            }

            // Update the ticket properties
            ticket.Title = request.Title;
            ticket.Description = request.Description;
            ticket.Department = request.Department;
            ticket.ResolvedBy = user.Id;
            ticket.ResolvedDate = DateTime.Now;

            await _context.SaveChangesAsync(cancellationToken);

            // Log the action in the audit trail
            var auditTrail = new Models.Domain.AuditTrail
            {
                Action = "Updated",
                TableAffected = "Tickets",
                Timestamp = DateTime.Now,
                ApplicationUserId = user.Id
            };

            _context.AuditTrails.Add(auditTrail);
            await _context.SaveChangesAsync(cancellationToken);

            // Get all users in "Technician" role
            var technicians = await _userManager.GetUsersInRoleAsync("Technician");

            // Notify technicians about the updated ticket
            foreach (var technician in technicians)
            {
                var notification = new Notification
                {
                    ApplicationUserId = technician.Id,
                    Message = $"Ticket \"{updatedTicket.Title}\" has been updated.",
                    Type = "TicketUpdated",
                    TicketId = updatedTicket.Id,
                    IsRead = false,
                    Timestamp = DateTime.Now
                };
                _context.Notifications.Add(notification);
            }

            await _context.SaveChangesAsync(cancellationToken);

            // Notify connected technicians in SignalR group
            await _hubContext.Clients.Group("Technicians").SendAsync("ReceiveTicketUpdate", updatedTicket);

            return true;
        }
    }
}
