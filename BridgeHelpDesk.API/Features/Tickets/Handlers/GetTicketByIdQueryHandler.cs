using BridgeHelpDesk.API.Data;
using BridgeHelpDesk.API.Features.Tickets.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BridgeHelpDesk.API.Features.Tickets.Handlers
{
    public class GetTicketByIdQueryHandler : IRequestHandler<GetTicketByIdQuery, Models.Domain.Ticket>
    {
        private readonly ApplicationDbContext _context;
        public GetTicketByIdQueryHandler(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Models.Domain.Ticket> Handle(GetTicketByIdQuery request, CancellationToken cancellationToken)
        {
            var ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.Id == request.TicketId, cancellationToken);

            return ticket;
        }
    }
}
