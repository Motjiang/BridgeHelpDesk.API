using MediatR;

namespace BridgeHelpDesk.API.Features.Notifications.Commands
{
    public record MarkNotificationAsReadCommand(int notificationId) : IRequest<bool>;
}
