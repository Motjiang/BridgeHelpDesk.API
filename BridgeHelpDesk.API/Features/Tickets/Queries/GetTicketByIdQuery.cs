using MediatR;

namespace BridgeHelpDesk.API.Features.Tickets.Queries
{
    public record GetTicketByIdQuery(int TicketId) : IRequest<Models.Domain.Ticket>;
}
