using BridgeHelpDesk.API.Models.DTOs.Dashboard;
using MediatR;

namespace BridgeHelpDesk.API.Features.Tickets.Queries
{
    public class TechnicianDashboardQuery() : IRequest<TechnicianDashboardDto>;
}
