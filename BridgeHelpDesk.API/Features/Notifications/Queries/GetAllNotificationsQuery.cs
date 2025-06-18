using BridgeHelpDesk.API.Models.Domain;
using MediatR;

namespace BridgeHelpDesk.API.Features.Notifications.Queries
{
    public record GetAllNotificationsQuery() : IRequest<IEnumerable<Notification>>;
}
