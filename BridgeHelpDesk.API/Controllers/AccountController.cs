using BridgeHelpDesk.API.Data;
using BridgeHelpDesk.API.Models.Domain;
using BridgeHelpDesk.API.Models.DTOs.Account;
using BridgeHelpDesk.API.Services;
using Mailjet.Client.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Security.Claims;
using System.Text;

namespace BridgeHelpDesk.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly JWTService _jwtService;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly EmailService _emailService;
        private readonly IConfiguration _config;

        public AccountController(JWTService jwtService, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, EmailService emailService, IConfiguration config)
        {
            _jwtService = jwtService;
            _signInManager = signInManager;
            _userManager = userManager;
            _emailService = emailService;
            _config = config;
        }


        [Authorize]
        [HttpGet("refresh-user-token")]
        public async Task<ActionResult<ApplicationUserDto>> RefreshUserToken()
        {
            var user = await _userManager.FindByNameAsync(User.FindFirst(ClaimTypes.Email)?.Value);

            if (await _userManager.IsLockedOutAsync(user))
            {
                return Unauthorized("You have been locked out");
            }
            return await CreateApplicationUserDto(user);
        }

        [HttpPost("login")]
        public async Task<ActionResult<ApplicationUserDto>> Login(LoginDto model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user == null) return Unauthorized("Invalid username or password");

            if (user.EmailConfirmed == false) return Unauthorized("Please confirm your email.");

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);

            if (result.IsLockedOut)
            {
                return Unauthorized(string.Format("Your account has been locked. You should wait until {0} (UTC time) to be able to login", user.LockoutEnd));
            }

            if (!result.Succeeded)
            {
                // User has input an invalid password
                if (!user.UserName.Equals(ApplicationDataSeed.TechnicianUserName))
                {
                    // Increamenting AccessFailedCount of the AspNetUser by 1
                    await _userManager.AccessFailedAsync(user);
                }

                if (user.AccessFailedCount >= ApplicationDataSeed.MaximumLoginAttempts)
                {
                    // Lock the user for one day
                    await _userManager.SetLockoutEndDateAsync(user, DateTime.UtcNow.AddDays(1));
                    return Unauthorized(string.Format("Your account has been locked. You should wait until {0} (UTC time) to be able to login", user.LockoutEnd));
                }


                return Unauthorized("Invalid username or password");
            }

            await _userManager.ResetAccessFailedCountAsync(user);
            await _userManager.SetLockoutEndDateAsync(user, null);

            return await CreateApplicationUserDto(user);
        }

        [HttpPost("forgot-username-or-password/{email}")]
        public async Task<IActionResult> ForgotUsernameOrPassword(string email)
        {
            if (string.IsNullOrEmpty(email)) return BadRequest("Invalid email");

            var user = await _userManager.FindByEmailAsync(email);

            if (user == null) return Unauthorized("This email address has not been registerd yet");
            if (user.EmailConfirmed == false) return BadRequest("Please confirm your email address first.");

            try
            {
                if (await SendForgotUsernameOrPasswordEmail(user))
                {
                    return Ok(new JsonResult(new { title = "Forgot username or password email sent", message = "Please check your email" }));
                }

                return BadRequest("Failed to send email. Please contact admin");
            }
            catch (Exception)
            {
                return BadRequest("Failed to send email. Please contact admin");
            }
        }


        [HttpPut("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) return Unauthorized("This email address has not been registerd yet");
            if (user.EmailConfirmed == false) return BadRequest("PLease confirm your email address first");

            try
            {
                var decodedTokenBytes = WebEncoders.Base64UrlDecode(model.Token);
                var decodedToken = Encoding.UTF8.GetString(decodedTokenBytes);

                var result = await _userManager.ResetPasswordAsync(user, decodedToken, model.NewPassword);
                if (result.Succeeded)
                {
                    return Ok(new JsonResult(new { title = "Password reset success", message = "Your password has been reset" }));
                }

                return BadRequest("Invalid token. Please try again");
            }
            catch (Exception)
            {
                return BadRequest("Invalid token. Please try again");
            }
        }


        #region
        private async Task<ApplicationUserDto> CreateApplicationUserDto(ApplicationUser user)
        {
            return new ApplicationUserDto
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                JWT = await _jwtService.CreateJWT(user),
            };
        }

        private async Task<bool> SendForgotUsernameOrPasswordEmail(ApplicationUser user)
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var url = $"{_config["JWT:ClientUrl"]}/{_config["Email:ResetPasswordPath"]}?token={token}&email={user.Email}";

            var body = $@"
                        <!DOCTYPE html>
                        <html>
                        <head>
                          <meta charset='UTF-8'>
                          <style>
                            body {{
                              font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
                              background-color: #f4f4f7;
                              color: #333333;
                              padding: 20px;
                            }}
                            .container {{
                              max-width: 600px;
                              margin: auto;
                              background-color: #ffffff;
                              border-radius: 10px;
                              box-shadow: 0 0 10px rgba(0, 0, 0, 0.05);
                              padding: 30px;
                            }}
                            .header {{
                              text-align: center;
                              background-color: #0d6efd;
                              color: white;
                              padding: 15px;
                              border-radius: 10px 10px 0 0;
                              font-size: 20px;
                            }}
                            .content {{
                              margin-top: 20px;
                            }}
                            .button {{
                              display: inline-block;
                              margin-top: 20px;
                              padding: 12px 25px;
                              background-color: #0d6efd;
                              color: white;
                              text-decoration: none;
                              border-radius: 5px;
                              font-weight: bold;
                            }}
                            .footer {{
                              margin-top: 30px;
                              font-size: 12px;
                              color: #777777;
                              text-align: center;
                            }}
                          </style>
                        </head>
                        <body>
                          <div class='container'>
                            <div class='header'>
                              Password Reset Request
                            </div>
                            <div class='content'>
                              <p>Hello <strong>{user.FirstName} {user.LastName}</strong>,</p>
                              <p>Your username is: <strong>{user.UserName}</strong></p>
                              <p>We received a request to reset your password. If this was you, click the button below:</p>
                              <p style='text-align:center;'>
                                <a class='button' href='{url}'>Reset Your Password</a>
                              </p>
                              <p>If you didn't request this, you can safely ignore this email.</p>
                              <p>Thank you,<br/><strong>{_config["Email:ApplicationName"]}</strong></p>
                            </div>
                            <div class='footer'>
                              &copy; {DateTime.Now.Year} {_config["Email:ApplicationName"]}. All rights reserved.
                            </div>
                          </div>
                        </body>
                        </html>
                        ";

            var emailSend = new EmailSendDto(user.Email, "Forgot username or password", body);

            return await _emailService.SendEmailAsync(emailSend);
        }
        #endregion
    }
}
