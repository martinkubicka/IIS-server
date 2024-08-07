/**
* @file LoginController.cs
* @author { Martin Kubicka (xkubic45) }
* @date 17.12.2023
* @brief Definition of login controller
*/

using System.Configuration;
using IIS_SERVER.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using IIS_SERVER.Login.Models;
using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace IIS_SERVER.Login.Controllers;

[ApiController]
[Route("[controller]")]
public class LoginController : ControllerBase, ILoginContoller
{
    private readonly IMySQLService MySqlService;
    public IConfiguration Configuration;

    public LoginController(IMySQLService mySqlService, IConfiguration configuration)
    {
        MySqlService = mySqlService;
        Configuration = configuration;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginModel data)
    {
        Tuple<bool, string?> result = await MySqlService.Login(data.Email, data.Password);
        if (result.Item1)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["jwt_secret"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var role = await MySqlService.GetUserRole(data.Email);
            var name = await MySqlService.GetUserHandle(data.Email);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Aud, Configuration["jwt_audience"]),
                new Claim(JwtRegisteredClaimNames.Iss, Configuration["jwt_issuer"]),
                new Claim(ClaimTypes.Role, role.Item1.ToString()),
                new Claim(ClaimTypes.Email, data.Email),
                new Claim(ClaimTypes.Name, name.Item1),
            };

            var token = new JwtSecurityToken(
                issuer: Configuration["jwt_issuer"],
                audience: Configuration["jwt_audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            
            Response.Cookies.Append("jwtToken", tokenString, new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.Now.AddMinutes(30),
                SameSite = SameSiteMode.None,
                Secure = false,
            });

            return StatusCode(200, tokenString);
        }
        else
        {
            return StatusCode(401, "Error: Invalid email or password.");   
        }
    }
    
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout() {
        Response.Cookies.Delete("jwtToken");
        return StatusCode(200, "Logout successful.");
    }
    
    [HttpPost("forgot")]
    public async Task<IActionResult> ForgotPassword(string email)
    {
        Tuple<bool, string?> result = await MySqlService.ForgotPassword(email);
        if (result.Item1)
        {
            try
            {
                SmtpClient client = new SmtpClient();
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.EnableSsl = true;
                client.Host = "smtp.gmail.com";
                client.Port = 587;

                System.Net.NetworkCredential credentials = 
                    new System.Net.NetworkCredential(Configuration["mail"], Configuration["mail_password"]);
                client.UseDefaultCredentials = false;
                client.Credentials = credentials;                

                MailMessage msg = new MailMessage();
                msg.From = new MailAddress(Configuration["mail"]);
                msg.To.Add(new MailAddress(email));

                msg.Subject = "Password reset";
                msg.IsBodyHtml = true;
                msg.Body = string.Format("<html><head></head><body><b> https://prod--dazzling-bunny-e92364.netlify.app/passwordReset?token=" + result.Item2 + "</b></body>");
                client.Send(msg);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return StatusCode(200, "Email sent.");
        }
        else
        {
            if (result.Item2.Contains("PRIMARY"))
            {
                return StatusCode(404, "Error: User with email not found.");
            }
            else
            {
                return StatusCode(500, "Error: " + result.Item2);
            }
        }
    }

    [HttpPut("newPassword")]
    public async Task<IActionResult> NewPassword([FromBody] NewPasswordModel data)
    {
        Tuple<bool, string?> result = await MySqlService.NewPassword(data);
        if (result.Item1)
        {
            return StatusCode(200, "Password updated successfully.");
        }
        else
        {
            if (result.Item2.Contains("Token"))
            {
                return StatusCode(404, "Error: Token not found.");
            }
            else
            {
                return StatusCode(500, "Error: " + result.Item2);
            }
        }
    }
}
