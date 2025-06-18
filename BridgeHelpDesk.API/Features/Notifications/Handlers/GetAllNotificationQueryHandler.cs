using BridgeHelpDesk.API.Data;
using BridgeHelpDesk.API.Features.Notifications.Queries;
using BridgeHelpDesk.API.Models.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BridgeHelpDesk.API.Features.Notifications.Handlers
{
    public class GetAllNotificationQueryHandler : IRequestHandler<GetAllNotificationsQuery, IEnumerable<Notification>>
    {
        private readonly ApplicationDbContext _context;
        public GetAllNotificationQueryHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Notification>> Handle(GetAllNotificationsQuery request, CancellationToken cancellationToken)
        {
            var notifications = await _context.Notifications.Where(n => n.IsRead == false).ToListAsync(cancellationToken);

            return notifications;
        }
    }
}
