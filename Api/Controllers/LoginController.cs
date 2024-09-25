using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Api.Database;
using Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
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
        private readonly IMemoryCache _memoryCache;

        public LoginController(DatabaseContext databaseContext, IConfiguration configuration, IMemoryCache memoryCache)
        {
            _databaseContext = databaseContext;
            _configuration = configuration;
            _memoryCache = memoryCache;
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

                // Generate both access token and refresh token
                string accessToken = GenerateJWTToken(request.Username);
                string refreshToken = GenerateRefreshToken();

                // Store refresh token in memory cache
                _memoryCache.Set(refreshToken, request.Username, TimeSpan.FromDays(7)); // 7-day expiry for refresh token

                return Ok(new 
                { 
                    AccessToken = accessToken,
                    RefreshToken = refreshToken
                });
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
        
        [HttpPost("refresh-token")]
        public IActionResult RefreshToken([FromBody] TokenModel model)
        {
            if (_memoryCache.TryGetValue(model.RefreshToken, out string username))
            {
                // Generate new access token
                string newAccessToken = GenerateJWTToken(username);

                // Optionally, you can invalidate the old refresh token and issue a new one:
                string newRefreshToken = GenerateRefreshToken();
                _memoryCache.Set(newRefreshToken, username, TimeSpan.FromDays(7)); // Store new refresh token
                _memoryCache.Remove(model.RefreshToken); // Remove old refresh token from cache

                return Ok(new 
                { 
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken
                });
            }

            return Unauthorized("Invalid or expired refresh token");
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
                expires: DateTime.Now.AddMinutes(30), // Access token expires in 30 minutes
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
    }
}
