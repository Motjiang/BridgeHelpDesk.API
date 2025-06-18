using BridgeHelpDesk.API.Data;
using BridgeHelpDesk.API.Features.Notifications.Commands;
using MediatR;

namespace BridgeHelpDesk.API.Features.Notifications.Handlers
{
    public class MarkNotificationAsReadCommandHandler : IRequestHandler<MarkNotificationAsReadCommand, bool>
    {
        private readonly ApplicationDbContext _context;

        public MarkNotificationAsReadCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(MarkNotificationAsReadCommand request, CancellationToken cancellationToken)
        {
            var notification = await _context.Notifications.FindAsync(request.notificationId);

            if (notification == null || notification.IsRead)
                return false;

            notification.IsRead = true;

            _context.Notifications.Update(notification);
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
