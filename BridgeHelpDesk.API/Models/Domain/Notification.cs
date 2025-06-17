using System.ComponentModel.DataAnnotations;

namespace BridgeHelpDesk.API.Models.Domain
{
    public class Notification
    {
        [Key]
        public int Id { get; set; }
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        public string Message { get; set; }
        public DateTime Timestamp { get; set; } 
        public bool IsRead { get; set; } = false;

        public string Type { get; set; } // e.g., "TicketCreated", "StatusChanged"
        public int? TicketId { get; set; }
    }
}
