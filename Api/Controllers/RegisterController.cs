using Api.Database;
using Api.Models;
using Api.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
	[Route("api/v1/")]
	[ApiController]
	public class RegisterController : ControllerBase
	{
		private readonly DatabaseContext _databaseContext;
		private readonly PasswordService _passwordService;

		public RegisterController(DatabaseContext databaseContext, PasswordService passwordService)
		{
			_databaseContext = databaseContext;
			_passwordService = passwordService;
		}

		[HttpPost("register")]
		public async Task<IActionResult> Register(RegisterModel model)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			if (await _databaseContext.Users.AnyAsync(x => x.Username == model.Username))
			{
				return BadRequest("Username is already taken!");
			}

			var user = new User
			{
				Username = model.Username,
				Email = model.Email,
				PasswordHash = _passwordService.HashPassword(new User(), model.Password),
				FailedAttempts = 0,
				LockoutEnd = null
			};

			_databaseContext.Users.Add(user);
			await _databaseContext.SaveChangesAsync();

			return Ok( new {Message = "User registered successfully"});
			
		}
	}
}
