using BridgeHelpDesk.API.Data;
using BridgeHelpDesk.API.Features.Tickets.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BridgeHelpDesk.API.Features.Tickets.Handlers
{
    public class GetAllTicketsQueryHandler : IRequestHandler<GetAllTicketsQuery, IEnumerable<Models.Domain.Ticket>>
    {
        private readonly ApplicationDbContext _context;

        public GetAllTicketsQueryHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Models.Domain.Ticket>> Handle(GetAllTicketsQuery request, CancellationToken cancellationToken)
        {
           return await _context.Tickets.ToListAsync(cancellationToken);
        }
    }
}
