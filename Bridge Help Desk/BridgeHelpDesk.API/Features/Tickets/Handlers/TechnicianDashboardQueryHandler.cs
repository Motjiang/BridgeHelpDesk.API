using BridgeHelpDesk.API.Data;
using BridgeHelpDesk.API.Features.Tickets.Queries;
using BridgeHelpDesk.API.Models.Domain;
using BridgeHelpDesk.API.Models.DTOs.Dashboard;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BridgeHelpDesk.API.Features.Tickets.Handlers
{
    public class TechnicianDashboardQueryHandler : IRequestHandler<TechnicianDashboardQuery, TechnicianDashboardDto>
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public TechnicianDashboardQueryHandler(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<TechnicianDashboardDto> Handle(TechnicianDashboardQuery request, CancellationToken cancellationToken)
        {
           var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);

            var totalTickets = await _context.Tickets.CountAsync(cancellationToken);
            var deletedTickets = await _context.Tickets.CountAsync(t => t.Status == "Removed", cancellationToken);
            var resolvedTickets = await _context.Tickets.CountAsync(t => t.ApplicationUserId == user.Id && t.Status == "Resolved", cancellationToken);

            return new TechnicianDashboardDto
            {
                TotalTicketsLogged = totalTickets,
                TotalTicketsDeleted = deletedTickets,
                TotalTicketsResolved = resolvedTickets
            };
        }
    }
}
