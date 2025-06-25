using MediatR;

namespace BridgeHelpDesk.API.Features.Tickets.Commands
{
    public record DeleteTicketCommand(int ticketId) : IRequest<bool>;
}
