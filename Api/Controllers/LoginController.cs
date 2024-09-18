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
		private readonly IConfiguration _configuration;
		private readonly TokenService _tokenService;

		public LoginController(DatabaseContext databaseContext, PasswordService passwordService, IConfiguration configuration, TokenService tokenService)
		{
			_databaseContext = databaseContext;
			_passwordService = passwordService;
			_configuration = configuration;
			_tokenService = tokenService;
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login(LoginModel model)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			var user = await _databaseContext.Users.SingleOrDefaultAsync(x => x.Username == model.Username);
            if (user == null || !_passwordService.VerifyPassword(user, model.Password))
            {
				return Unauthorized("Invalid username or password");
            }

			var token = _tokenService.GenerateJwtToken(user);
			return Ok(new {Token = token});
        }
	}
}
