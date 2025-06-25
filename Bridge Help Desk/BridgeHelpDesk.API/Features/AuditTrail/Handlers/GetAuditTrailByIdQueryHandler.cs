using BridgeHelpDesk.API.Data;
using BridgeHelpDesk.API.Features.AuditTrail.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BridgeHelpDesk.API.Features.AuditTrail.Handlers
{
    public class GetAuditTrailByIdQueryHandler : IRequestHandler<GetAuditTrailByIdQuery, Models.Domain.AuditTrail>
    {
        private readonly ApplicationDbContext _context;

        public GetAuditTrailByIdQueryHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Models.Domain.AuditTrail> Handle(GetAuditTrailByIdQuery request, CancellationToken cancellationToken)
        {
            var auditTrail = await _context.AuditTrails
                .FirstOrDefaultAsync(a => a.Id == request.AuditTrailId, cancellationToken);

            return auditTrail;
        }
    }
}
