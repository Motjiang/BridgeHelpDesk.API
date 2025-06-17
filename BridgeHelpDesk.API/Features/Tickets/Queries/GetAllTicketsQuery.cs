using MediatR;

namespace BridgeHelpDesk.API.Features.Tickets.Queries
{
    public class GetAllTicketsQuery() : IRequest<IEnumerable<Models.Domain.Ticket>>;
}
