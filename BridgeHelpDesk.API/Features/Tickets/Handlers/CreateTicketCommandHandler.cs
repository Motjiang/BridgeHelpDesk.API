using BridgeHelpDesk.API.Data;
using BridgeHelpDesk.API.Features.Ticket.Commands;
using BridgeHelpDesk.API.Hubs.Ticket;
using BridgeHelpDesk.API.Models.Domain;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using System.Data;

namespace BridgeHelpDesk.API.Features.Ticket.Handlers
{
    public class CreateTicketCommandHandler : IRequestHandler<CreateTicketCommand, int>
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<TicketHub> _hubContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CreateTicketCommandHandler(
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


        public async Task<int> Handle(CreateTicketCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            var ticket = new Models.Domain.Ticket
            {
                Title = request.Title,
                Description = request.Description,
                ApplicationUserId = user.Id,
                Department = request.Department,
                Status = "Open",
                CreatedAt = DateTime.Now
            };
            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync(cancellationToken);

            // Optionally, you can log an audit trail here if needed
            var auditTrail = new Models.Domain.AuditTrail
            {                
                Action = "Created",
                TableAffected = "Tickets",
                Timestamp = DateTime.Now,
                ApplicationUserId = user.Id,
            };

            _context.AuditTrails.Add(auditTrail);
            await _context.SaveChangesAsync(cancellationToken);

            // Get all users in "Technician" role
            var technicians = await _userManager.GetUsersInRoleAsync("Technician");

            // notify technicians about the new ticket
            foreach (var technician in technicians)
            {
                var notification = new Notification
                {
                    ApplicationUserId = technician.Id,
                    Message = $"New ticket \"{ticket.Title}\" logged.",
                    Type = "TicketCreated",
                    TicketId = ticket.Id,
                    IsRead = false,
                    Timestamp = DateTime.Now
                };
                _context.Notifications.Add(notification);
            }

            await _context.SaveChangesAsync(cancellationToken);

            // Notify connected technicians in SignalR group
            await _hubContext.Clients.Group("Technicians").SendAsync("TicketCreated", ticket);

            return ticket.Id;
        }
    }
}
