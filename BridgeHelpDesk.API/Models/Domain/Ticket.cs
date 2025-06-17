using System.ComponentModel.DataAnnotations;

namespace BridgeHelpDesk.API.Models.Domain
{
    public class Ticket
    {
        [Key]   
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public string Department { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ResolvedDate { get; set; }
        public string? ResolvedBy { get; set; }
    }
}
