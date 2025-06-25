using BridgeHelpDesk.API.Models.Domain;
using MediatR;

namespace BridgeHelpDesk.API.Features.Notifications.Queries
{
    public record GetAllNotificationForAllQuery : IRequest<IEnumerable<Notification>>;
}
