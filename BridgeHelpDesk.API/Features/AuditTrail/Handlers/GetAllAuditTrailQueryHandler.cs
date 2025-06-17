using BridgeHelpDesk.API.Data;
using BridgeHelpDesk.API.Features.AuditTrail.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BridgeHelpDesk.API.Features.AuditTrail.Handlers
{
    public class GetAllAuditTrailQueryHandler : IRequestHandler<GetAllAuditTrailsQuery, IEnumerable<Models.Domain.AuditTrail>>
    {
        private readonly ApplicationDbContext _context;

        public GetAllAuditTrailQueryHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Models.Domain.AuditTrail>> Handle(GetAllAuditTrailsQuery request, CancellationToken cancellationToken)
        {
            return await _context.AuditTrails.ToListAsync(cancellationToken);
        }
    }
}
