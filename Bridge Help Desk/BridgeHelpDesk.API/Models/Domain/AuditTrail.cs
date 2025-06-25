using System.ComponentModel.DataAnnotations;

namespace BridgeHelpDesk.API.Models.Domain
{
    public class AuditTrail
    {
        [Key]
        public int Id { get; set; }
        public string Action { get; set; }
        public string TableAffected { get; set; }
        public DateTime Timestamp { get; set; }
        public string ApplicationUserId { get; set; } 
        public ApplicationUser ApplicationUser { get; set; }
    }
}
