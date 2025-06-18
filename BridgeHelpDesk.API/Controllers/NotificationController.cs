using BridgeHelpDesk.API.Features.Notifications.Commands;
using BridgeHelpDesk.API.Features.Notifications.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BridgeHelpDesk.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly ISender _mediator;

        public NotificationController(ISender mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("get-all-notifications")]
        public async Task<IActionResult> GetAllNotifications()
        {
            var notifications = await _mediator.Send(new GetAllNotificationsQuery());

            if (notifications == null || !notifications.Any())
                return NotFound("No notifications found.");

            return Ok(notifications);
        }

        [HttpPatch("mark-as-read/{id}")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var result = await _mediator.Send(new MarkNotificationAsReadCommand(id));

            if (!result)
                return NotFound("Notification not found or already marked as read.");

            return Ok();
        }
    }
}
