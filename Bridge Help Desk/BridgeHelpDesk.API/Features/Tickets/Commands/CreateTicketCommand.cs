using BridgeHelpDesk.API.Models.Domain;
using MediatR;

namespace BridgeHelpDesk.API.Features.Ticket.Commands
{
    public record CreateTicketCommand(string Title, string Description, string Department) : IRequest<int>;    
}
