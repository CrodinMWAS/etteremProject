using Api.Database;
using Api.Models;
using Api.Service;
using Api.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using MySql.Data.MySqlClient;

namespace Api.Controllers
{
    [Route("api/v1/")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly DatabaseContext _databaseContext;
        private readonly TokenService _tokenService;
        private readonly LoginHelper _loginHelper;
        private readonly IMemoryCache _memoryCache;

        public LoginController(DatabaseContext databaseContext, TokenService tokenService, LoginHelper loginHelper ,IMemoryCache memoryCache)
        {
            _databaseContext = databaseContext;
            _tokenService = tokenService;
            _loginHelper = loginHelper;
            _memoryCache = memoryCache;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel request)
        {
            string ipAddress = HttpContext.Connection.RemoteIpAddress.ToString();

            if (await _loginHelper.IsIpTimedOutAsync(ipAddress))
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
                await _loginHelper.ResetLoginAttemptsAsync(ipAddress);

                string accessToken = _tokenService.GenerateJWTToken(request.Username);
                string refreshToken = _tokenService.GenerateRefreshToken();

                _memoryCache.Set(refreshToken, request.Username,
                    TimeSpan.FromDays(7)); // 7-day expiry for refresh token

                return Ok(new
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken
                });
            }

            await _loginHelper.IncrementLoginAttemptsAsync(ipAddress);
            return BadRequest("Invalid username or password");
        }
    }
}
