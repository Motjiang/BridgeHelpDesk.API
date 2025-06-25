using BridgeHelpDesk.API.Data;
using BridgeHelpDesk.API.Features.Notifications.Queries;
using BridgeHelpDesk.API.Models.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BridgeHelpDesk.API.Features.Notifications.Handlers
{
    public class GetAllNotificationForAllQueryHandler : IRequestHandler<GetAllNotificationForAllQuery, IEnumerable<Notification>>
    {
        private readonly ApplicationDbContext _context;

        public GetAllNotificationForAllQueryHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Notification>> Handle(GetAllNotificationForAllQuery request, CancellationToken cancellationToken)
        {
            var notifications = await _context.Notifications.Where(t => t.Type != "TicketCreated"
            && t.Type != "TicketUpdated" && t.Type != "TicketDeleted").ToListAsync(cancellationToken);

            return notifications;
        }
    }
}
