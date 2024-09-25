using Api.Database;
using Api.Models;
using Api.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
	[Route("api/v1/")]
	[ApiController]
	public class LoginController : ControllerBase
	{
		private readonly DatabaseContext _databaseContext;
		private readonly PasswordService _passwordService;
		private readonly TokenService _tokenService;
		private readonly ILogger<LoginController> _logger;

		public LoginController(DatabaseContext databaseContext, PasswordService passwordService, TokenService tokenService, ILogger<LoginController> logger)
		{
			_databaseContext = databaseContext;
			_passwordService = passwordService;
			_tokenService = tokenService;
			_logger = logger;
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login(LoginModel model)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			var user = await _databaseContext.Users.SingleOrDefaultAsync(x => x.Username == model.Username);
			
			if (user == null)
			{
				return Unauthorized("Invalid username or password");
			}

			// if (user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.UtcNow)
			// {
			// 	return Unauthorized("Account is locked. Try again later!");
			// }

			if (!_passwordService.VerifyPassword(user, model.Password))
            {
				// user.FailedAttempts++;
				// if (user.FailedAttempts >= 5)
				// {
				// 	user.LockoutEnd = DateTime.UtcNow.AddMinutes(5);
				// }
				// await _databaseContext.SaveChangesAsync();
				_logger.LogWarning("Failed login attempt for user {Username} from IP {IP}", model.Username, HttpContext.Connection.RemoteIpAddress?.ToString());

				return Unauthorized("Invalid username or password");
            }

			// user.FailedAttempts = 0;
			// user.LockoutEnd = null;
			await _databaseContext.SaveChangesAsync();

			var token = _tokenService.GenerateJwtToken(user);
			return Ok(new {Token = token});
        }
	}
}
