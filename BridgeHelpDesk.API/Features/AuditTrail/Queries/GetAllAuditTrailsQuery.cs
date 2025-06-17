using MediatR;

namespace BridgeHelpDesk.API.Features.AuditTrail.Queries
{
    public record GetAllAuditTrailsQuery() : IRequest<IEnumerable<Models.Domain.AuditTrail>>;
}   
