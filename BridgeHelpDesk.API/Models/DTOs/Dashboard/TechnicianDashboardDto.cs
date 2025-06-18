namespace BridgeHelpDesk.API.Models.DTOs.Dashboard
{
    public class TechnicianDashboardDto
    {
        public int TotalTicketsLogged { get; set; }
        public int TotalTicketsResolved { get; set; }
        public int TotalTicketsDeleted { get; set; }
    }
}
