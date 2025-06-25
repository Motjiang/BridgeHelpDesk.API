using BridgeHelpDesk.API.Controllers;
using BridgeHelpDesk.API.Features.Notifications.Commands;
using BridgeHelpDesk.API.Features.Notifications.Queries;
using BridgeHelpDesk.API.Models.Domain;
using FakeItEasy;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BridgeHelpDesk.UnitTests.Controllers.Notification
{
    public class NotificationControllerTests
    {
        private readonly ISender _mediator;
        private readonly NotificationController _controller;

        public NotificationControllerTests()
        {
            _mediator = A.Fake<ISender>();
            _controller = new NotificationController(_mediator);
        }


        [Fact]
        public async Task GetAllNotifications_ReturnsOk_WhenNotificationsExist()
        {
            // Arrange
            var notifications = new List<API.Models.Domain.Notification>
            {
                new API.Models.Domain.Notification {
                    Id = 1,
                    Message = "Ticket logged",
                    ApplicationUserId = "user-1",
                    Timestamp = DateTime.UtcNow,
                    IsRead = false,
                    Type = "TicketCreated",
                    TicketId = 101
                },
                new API.Models.Domain.Notification {
                    Id = 2,
                    Message = "Ticket resolved",
                    ApplicationUserId = "user-2",
                    Timestamp = DateTime.UtcNow,
                    IsRead = false,
                    Type = "StatusChanged",
                    TicketId = 102
                }
            };

            A.CallTo(() => _mediator.Send(A<GetAllNotificationsQuery>._, A<CancellationToken>._))
                .Returns(Task.FromResult<IEnumerable<API.Models.Domain.Notification>>(notifications));

            // Act
            var result = await _controller.GetAllNotifications();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult!.Value.Should().BeEquivalentTo(notifications);
        }

        [Fact]
        public async Task GetAllNotifications_ReturnsNotFound_WhenNoNotifications()
        {
            // Arrange
            A.CallTo(() => _mediator.Send(A<GetAllNotificationsQuery>._, A<CancellationToken>._))
                .Returns(new List<API.Models.Domain.Notification>());

            // Act
            var result = await _controller.GetAllNotifications();

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("No notifications found.", notFoundResult.Value);
        }

        [Fact]
        public async Task MarkAsRead_ReturnsOk_WhenNotificationMarked()
        {
            // Arrange
            int id = 1;
            A.CallTo(() => _mediator.Send(A<MarkNotificationAsReadCommand>.That.Matches(c => c.notificationId == id), A<CancellationToken>._))
                .Returns(true);

            // Act
            var result = await _controller.MarkAsRead(id);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task MarkAsRead_ReturnsNotFound_WhenNotificationNotFound()
        {
            // Arrange
            int id = 1;
            A.CallTo(() => _mediator.Send(A<MarkNotificationAsReadCommand>.That.Matches(c => c.notificationId == id), A<CancellationToken>._))
                .Returns(false);

            // Act
            var result = await _controller.MarkAsRead(id);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Notification not found or already marked as read.", notFoundResult.Value);
        }
    }
}
