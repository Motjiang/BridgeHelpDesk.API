namespace BridgeHelpDesk.API.Data
{
    public class ApplicationDataSeed
    {
        // Roles
        public const string TechnicianRole = "Technician";
        public const string AssistantRole = "Assistant";

        // Default Users
        public const string TechnicianUserName = "technician@gmail.com";
        public const string AssitantUserName = "assistant@gmail.com";

        // Login Attempts
        public const int MaximumLoginAttempts = 3;
    }
}
