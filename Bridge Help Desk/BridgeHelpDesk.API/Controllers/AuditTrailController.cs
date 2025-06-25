using BridgeHelpDesk.API.Features.AuditTrail.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BridgeHelpDesk.API.Controllers
{
    [Authorize(Roles = "Technician")]
    [Route("api/[controller]")]
    [ApiController]
    public class AuditTrailController : ControllerBase
    {
        private readonly ISender _mediator;

        public AuditTrailController(ISender mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("get-audit-trails")]
        public async Task<IActionResult> GetAuditTrails()
        {
            var auditTrails = await _mediator.Send(new GetAllAuditTrailsQuery());

            if (auditTrails == null || !auditTrails.Any())
                return NotFound("No audit trails found.");

            return Ok(auditTrails);
        }

        [HttpGet("get-audit-trail/{auditTrailId}")]
        public async Task<IActionResult> GetAuditTrailById(int auditTrailId)
        {
            var auditTrail = await _mediator.Send(new GetAuditTrailByIdQuery(auditTrailId));

            if (auditTrail == null)
                return NotFound($"Audit trail with ID {auditTrailId} not found.");

            return Ok(auditTrail);
        }
    }
}
