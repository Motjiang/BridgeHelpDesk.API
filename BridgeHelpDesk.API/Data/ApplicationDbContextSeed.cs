using BridgeHelpDesk.API.Models.Domain;
using Mailjet.Client.Resources;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BridgeHelpDesk.API.Data
{
    public class ApplicationDbContextSeed
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public ApplicationDbContextSeed(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task GenerateContextAsync()
        {
            if (_context.Database.GetPendingMigrationsAsync().GetAwaiter().GetResult().Count() > 0)
            {
                // applies any pending migration into our database
                await _context.Database.MigrateAsync();
            }

            if (!_roleManager.Roles.Any())
            {
                await _roleManager.CreateAsync(new IdentityRole { Name = ApplicationDataSeed.TechnicianRole });
                await _roleManager.CreateAsync(new IdentityRole { Name = ApplicationDataSeed.AssistantRole });
            }

            if (!_userManager.Users.AnyAsync().GetAwaiter().GetResult())
            {
                var technician = new ApplicationUser
                {
                    FirstName = "Obakeng",
                    LastName = "Walker",
                    UserName = ApplicationDataSeed.TechnicianUserName,
                    Email = ApplicationDataSeed.TechnicianUserName,
                    EmailConfirmed = true,
                    DateCreated = DateTime.Now
                };
                await _userManager.CreateAsync(technician, "123456");
                await _userManager.AddToRoleAsync(technician, ApplicationDataSeed.TechnicianRole);
                await _userManager.AddClaimsAsync(technician, new Claim[]
                {
                    new Claim(ClaimTypes.Email, technician.Email),
                    new Claim(ClaimTypes.Surname, technician.LastName)
                });

                var assistant = new ApplicationUser
                {
                    FirstName = "Oneilwe",
                    LastName = "Snow",
                    UserName = ApplicationDataSeed.AssitantUserName,
                    Email = ApplicationDataSeed.AssitantUserName,
                    EmailConfirmed = true,
                    DateCreated = DateTime.Now
                };
                await _userManager.CreateAsync(assistant, "123456");
                await _userManager.AddToRoleAsync(assistant, ApplicationDataSeed.AssistantRole);
                await _userManager.AddClaimsAsync(assistant, new Claim[]
                {
                    new Claim(ClaimTypes.Email, assistant.Email),
                    new Claim(ClaimTypes.Surname, assistant.LastName)
                });
            }
        }
    }
}
