using MediatR;

namespace BridgeHelpDesk.API.Features.Tickets.Queries
{
    public record GetAllTicketsQuery() : IRequest<IEnumerable<Models.Domain.Ticket>>;
}
