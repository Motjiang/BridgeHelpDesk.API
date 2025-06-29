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
    public record DeleteTicketCommandHandler : IRequestHandler<DeleteTicketCommand, bool>
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<TicketHub> _hubContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public DeleteTicketCommandHandler(
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
        public async Task<bool> Handle(DeleteTicketCommand request, CancellationToken cancellationToken)
        {
            var ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.Id == request.ticketId, cancellationToken);

            var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);

            ticket.Status = "Deleted";
            ticket.RemovedBy = user.Id;
            ticket.RemovedDate = DateTime.Now;

            await _context.SaveChangesAsync(cancellationToken);

            // Log the action in the audit trail
            var auditTrail = new Models.Domain.AuditTrail
            {
                Action = "Deleted",
                TableAffected = "Tickets",
                Timestamp = DateTime.Now,
                ApplicationUserId = user.Id
            };

            _context.AuditTrails.Add(auditTrail);
            await _context.SaveChangesAsync(cancellationToken);

            // Get all users in "Technician" role
            var technicians = await _userManager.GetUsersInRoleAsync("Technician");

            // Notify all technicians about the ticket deletion
            foreach (var technician in technicians)
            {
                var notification = new Notification
                {
                    ApplicationUserId = technician.Id,
                    Message = $"Ticket \"{deletedTicket.Title}\" has been deleted.",
                    Type = "TicketDeleted",
                    TicketId = deletedTicket.Id,
                    IsRead = false,
                    Timestamp = DateTime.Now
                };
                _context.Notifications.Add(notification);
            }

            await _context.SaveChangesAsync(cancellationToken);

            // Notify all connected clients about the ticket deletion
            await _hubContext.Clients.All.SendAsync("TicketDeleted", deletedTicket);

            return true;
        }
    }
}
