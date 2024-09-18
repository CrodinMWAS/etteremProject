using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Api.Models;

namespace Api.Service
{
	public class TokenService
	{
		private readonly IConfiguration _configuration;

		public TokenService(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public string GenerateJwtToken(User user)
		{
			//var claims = new[]
			//{
			//	new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
			//	new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
			//};

			//var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]));
			//var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			//var token = new JwtSecurityToken(
			//	issuer: _configuration["JwtSettings:Issuer"],
			//	audience: _configuration["JwtSettings:Audience"],
			//	claims: claims,
			//	expires: DateTime.Now.AddMinutes(30),
			//	signingCredentials: creds
			//);

			//return new JwtSecurityTokenHandler().WriteToken(token);
			return "";
		}
	}
}
