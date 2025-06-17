using BridgeHelpDesk.API.Data;
using BridgeHelpDesk.API.Hubs.Ticket;
using BridgeHelpDesk.API.Models.Domain;
using BridgeHelpDesk.API.Services;
using Mailjet.Client.Resources;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddHttpContextAccessor();

// Register MediatR for CQRS pattern
builder.Services.AddMediatR(configuration =>
    configuration.RegisterServicesFromAssembly(typeof(Program).Assembly));

// Register SignalR for real-time communication
builder.Services.AddSignalR();

// Inject JWTService, EmailService and  ApplicationDbcontextSeed class inside our Controllers
builder.Services.AddScoped<JWTService>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<ApplicationDbContextSeed>();

// defining our IdentityCore Service
builder.Services.AddIdentityCore<ApplicationUser>(options =>
{
    // password configuration
    options.Password.RequiredLength = 6;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;

    // for email confirmation
    options.SignIn.RequireConfirmedEmail = true;
})
    .AddRoles<IdentityRole>() 
    .AddRoleManager<RoleManager<IdentityRole>>() 
    .AddEntityFrameworkStores<ApplicationDbContext>() 
    .AddSignInManager<SignInManager<ApplicationUser>>() 
    .AddUserManager<UserManager<ApplicationUser>>() 
    .AddDefaultTokenProviders(); 

// be able to authenticate users using JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // validate the token based on the key we have provided inside appsettings.development.json JWT:Key
            ValidateIssuerSigningKey = true,
            // the issuer singning key based on JWT:Key
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"])),
            // the issuer which in here is the api project url we are using
            ValidIssuer = builder.Configuration["JWT:Issuer"],
            // validate the issuer (who ever is issuing the JWT)
            ValidateIssuer = true,
            // validate audience 
            ValidateAudience = false
        };
    });

builder.Services.AddCors();


var app = builder.Build();

app.UseCors(opt =>
{
    opt.AllowAnyHeader().AllowAnyMethod().AllowCredentials().WithOrigins(builder.Configuration["JWT:ClientUrl"]);
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

#region ContextSeed
using var scope = app.Services.CreateScope();
try
{
    var contextSeedService = scope.ServiceProvider.GetService<ApplicationDbContextSeed>();
    await contextSeedService.GenerateContextAsync();
}
catch (Exception)
{
    throw new Exception("An error occurred while seeding the database.");
}
#endregion

// Map SignalR hubs
app.MapHub<TicketHub>("/ticketHub");

app.Run();
