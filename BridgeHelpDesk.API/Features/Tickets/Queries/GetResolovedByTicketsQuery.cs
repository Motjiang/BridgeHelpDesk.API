using MediatR;

namespace BridgeHelpDesk.API.Features.Tickets.Queries
{
    public class GetResolovedByTicketsQuery() : IRequest<List<Models.Domain.Ticket>>;
}
