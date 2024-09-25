using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Api.Database;
using Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MySql.Data.MySqlClient;

namespace Api.Controllers
{
    [Route("api/v1/")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly DatabaseContext _databaseContext;
        private readonly IConfiguration _configuration;

        public LoginController(DatabaseContext databaseContext, IConfiguration configuration)
        {
            _databaseContext = databaseContext;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel request)
        {
            string ipAddress = HttpContext.Connection.RemoteIpAddress.ToString();

            if (await IsIpTimedOutAsync(ipAddress))
            {
                return BadRequest("Timed out for 5 minutes, try again later");
            }

            var query = "SELECT Password FROM users WHERE UserName = @UserName";
            string storedHash = null;

            var command = _databaseContext.Database.GetDbConnection().CreateCommand();
            command.CommandText = query;
            command.Parameters.Add(new MySqlParameter("@UserName", request.Username));

            await _databaseContext.Database.OpenConnectionAsync();
            var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                storedHash = reader.GetString(0);
            }
            await _databaseContext.Database.CloseConnectionAsync();

            if (storedHash != null && BCrypt.Net.BCrypt.Verify(request.Password, storedHash))
            {
                await ResetLoginAttemptsAsync(ipAddress);
                string token = GenerateJWTToken(request.Username);
                return Ok(new { Token = token });
            }

            await IncrementLoginAttemptsAsync(ipAddress);
            return BadRequest("Invalid username or password");
        }

        private async Task IncrementLoginAttemptsAsync(string ipAddress)
        {
            var query = "SELECT AttemptCount, LastAttempt FROM loginattempts WHERE IPAddress = @IPAddress";
            int attemptCount = 0;
            DateTime? lastAttempt = null;

            var command = _databaseContext.Database.GetDbConnection().CreateCommand();
            command.CommandText = query;
            command.Parameters.Add(new MySqlParameter("@IPAddress", ipAddress));

            await _databaseContext.Database.OpenConnectionAsync();
            var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                attemptCount = reader.GetInt32(0);
                lastAttempt = reader.GetDateTime(1);
            }
            await _databaseContext.Database.CloseConnectionAsync();

            if (lastAttempt != null && (DateTime.Now - lastAttempt.Value).TotalMinutes < 5)
            {
                attemptCount++;
            }
            else
            {
                attemptCount = 1;
            }

            if (attemptCount >= 5)
            {
                return; // Already banned, no need to update further
            }

            await UpdateLoginAttemptAsync(ipAddress, attemptCount);
        }

        private async Task UpdateLoginAttemptAsync(string ipAddress, int attemptCount)
        {
            var query = "UPDATE LoginAttempts SET AttemptCount = @AttemptCount, LastAttempt = @LastAttempt WHERE IPAddress = @IPAddress";
            var command = _databaseContext.Database.GetDbConnection().CreateCommand();
            command.CommandText = query;
            command.Parameters.Add(new MySqlParameter("@AttemptCount", attemptCount));
            command.Parameters.Add(new MySqlParameter("@LastAttempt", DateTime.Now));
            command.Parameters.Add(new MySqlParameter("@IPAddress", ipAddress));

            await _databaseContext.Database.OpenConnectionAsync();
            await command.ExecuteNonQueryAsync();
            await _databaseContext.Database.CloseConnectionAsync();
        }

        private async Task ResetLoginAttemptsAsync(string ipAddress)
        {
            var query = "DELETE FROM LoginAttempts WHERE IPAddress = @IPAddress";
            var command = _databaseContext.Database.GetDbConnection().CreateCommand();
            command.CommandText = query;
            command.Parameters.Add(new MySqlParameter("@IPAddress", ipAddress));

            await _databaseContext.Database.OpenConnectionAsync();
            await command.ExecuteNonQueryAsync();
            await _databaseContext.Database.CloseConnectionAsync();
        }

        private async Task<bool> IsIpTimedOutAsync(string ipAddress)
        {
            var query = "SELECT AttemptCount, LastAttempt FROM LoginAttempts WHERE IPAddress = @IPAddress";
            int attemptCount = 0;
            DateTime? lastAttempt = null;

            var command = _databaseContext.Database.GetDbConnection().CreateCommand();
            command.CommandText = query;
            command.Parameters.Add(new MySqlParameter("@IPAddress", ipAddress));

            await _databaseContext.Database.OpenConnectionAsync();
            var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                attemptCount = reader.GetInt32(0);
                lastAttempt = reader.GetDateTime(1);
            }
            await _databaseContext.Database.CloseConnectionAsync();

            if (attemptCount >= 5 && (DateTime.Now - lastAttempt.Value).TotalMinutes < 5)
            {
                return true;
            }

            return false;
        }

        private string GenerateJWTToken(string username)
        {
            var secretKey = _configuration["JwtSettings:SecretKey"];
            var issuer = _configuration["JwtSettings:Issuer"];
            var audience = _configuration["JwtSettings:Audience"];
    
            if (string.IsNullOrEmpty(secretKey))
            {
                throw new ArgumentNullException(nameof(secretKey), "JWT SecretKey is not configured properly.");
            }
            
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
