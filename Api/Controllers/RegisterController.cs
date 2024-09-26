using System.Data;
using Api.Database;
using Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;

namespace Api.Controllers
{
	[Route("api/v1/")]
	[ApiController]
	public class RegisterController : ControllerBase
	{
		private readonly DatabaseContext _databaseContext;

		public RegisterController(DatabaseContext databaseContext, IConfiguration configuration)
		{
			_databaseContext = databaseContext;
		}

		[HttpPost("register")]
		public async Task<IActionResult> Register([FromBody] RegisterModel request)
		{
			// Check if the user already exists
			if (UserExists(request.Username))
			{
				return BadRequest("User already exists");
			}

			// Hash the user's password
			string hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

			// Insert new user into the database
			var query = "INSERT INTO users (UserName, Email, Password, IsAdmin) VALUES (@UserName, @Email, @Password, @IsAdmin)";
			var command = _databaseContext.Database.GetDbConnection().CreateCommand();
			command.CommandText = query;

			// Use MySqlParameter instead of SqlParameter
			command.Parameters.Add(new MySqlParameter("@UserName", request.Username));
			command.Parameters.Add(new MySqlParameter("@Password", hashedPassword));
			command.Parameters.Add(new MySqlParameter("@Email", request.Email));
			command.Parameters.Add(new MySqlParameter("@IsAdmin", false));

			await _databaseContext.Database.OpenConnectionAsync();
			await command.ExecuteNonQueryAsync();
			await _databaseContext.Database.CloseConnectionAsync();

			return Ok("User registered successfully");
		}

		private bool UserExists(string username)
		{
			var query = "SELECT 1 FROM users WHERE UserName = @UserName";
			bool userExists;

			var command = _databaseContext.Database.GetDbConnection().CreateCommand();
			command.CommandText = query;

			// Use MySqlParameter instead of SqlParameter
			command.Parameters.Add(new MySqlParameter("@username", username));

			_databaseContext.Database.OpenConnection();
			userExists = command.ExecuteScalar() != null;
			_databaseContext.Database.CloseConnection();

			return userExists;
		}
	}
}
