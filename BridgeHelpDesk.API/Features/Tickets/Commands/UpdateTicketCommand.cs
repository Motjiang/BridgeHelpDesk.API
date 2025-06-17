using MediatR;

namespace BridgeHelpDesk.API.Features.Tickets.Commands
{
    public record UpdateTicketCommand(int TicketId, string Title, string Description, string Department, string Status) : IRequest<bool>;
}
