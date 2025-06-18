using BridgeHelpDesk.API.Data;
using BridgeHelpDesk.API.Features.Tickets.Queries;
using BridgeHelpDesk.API.Models.Domain;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BridgeHelpDesk.API.Features.Tickets.Handlers
{
    public class GetResolvedByTicketQueryHandler : IRequestHandler<GetResolovedByTicketsQuery, List<Models.Domain.Ticket>>
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetResolvedByTicketQueryHandler(IHttpContextAccessor httpContextAccessor,
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<Models.Domain.Ticket>> Handle(GetResolovedByTicketsQuery request, CancellationToken cancellationToken)
        {
            var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);

            var tickets = await _context.Tickets.Where(t => t.ResolvedBy == user.Id && t.Status == "Resolved").ToListAsync(cancellationToken);

            return tickets;
        }
    }
}
