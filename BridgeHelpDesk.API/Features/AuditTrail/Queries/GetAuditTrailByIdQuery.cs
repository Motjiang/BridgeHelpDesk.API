using MediatR;

namespace BridgeHelpDesk.API.Features.AuditTrail.Queries
{
    public record GetAuditTrailByIdQuery(int AuditTrailId) : IRequest<Models.Domain.AuditTrail>;
}
