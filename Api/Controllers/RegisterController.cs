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
		private readonly IConfiguration _configuration;

		public RegisterController(DatabaseContext databaseContext, IConfiguration configuration)
		{
			_databaseContext = databaseContext;
			_configuration = configuration;
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
			var query = "INSERT INTO users (UserName, Email, Password, isAdmin) VALUES (@UserName, @Email, @Password, @isAdmin)";
			var command = _databaseContext.Database.GetDbConnection().CreateCommand();
			command.CommandText = query;

			// Use MySqlParameter instead of SqlParameter
			command.Parameters.Add(new MySqlParameter("@UserName", request.Username));
			command.Parameters.Add(new MySqlParameter("@Password", hashedPassword));
			command.Parameters.Add(new MySqlParameter("@Email", request.Email));
			command.Parameters.Add(new MySqlParameter("@isAdmin", false));

			await _databaseContext.Database.OpenConnectionAsync();
			await command.ExecuteNonQueryAsync();
			await _databaseContext.Database.CloseConnectionAsync();

			return Ok("User registered successfully");
		}

		private bool UserExists(string username)
		{
			var query = "SELECT 1 FROM users WHERE UserName = @username";
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
