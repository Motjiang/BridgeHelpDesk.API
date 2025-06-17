using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace BridgeHelpDesk.API.Models.Domain
{
    public class ApplicationUser : IdentityUser
    {

        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public DateTime DateCreated { get; set; }

        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    }
}
