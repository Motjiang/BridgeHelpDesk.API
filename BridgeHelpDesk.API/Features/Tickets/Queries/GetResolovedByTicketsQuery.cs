using MediatR;

namespace BridgeHelpDesk.API.Features.Tickets.Queries
{
    public record GetResolovedByTicketsQuery() : IRequest<List<Models.Domain.Ticket>>;
}
