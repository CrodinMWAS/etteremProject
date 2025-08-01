﻿using Microsoft.AspNet.Identity;
using Api.Models;
using Microsoft.AspNetCore.Identity;

namespace Api.Service
{
	public class PasswordService
	{
		private readonly PasswordHasher<User> _passwordHasher;

		public PasswordService()
		{
			_passwordHasher = new PasswordHasher<User>();
		}
		public string HashPassword(User user, string password)
		{
			return _passwordHasher.HashPassword(user, password);
		}

		public bool VerifyPassword(User user, string password)
		{
			var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
			return result == Microsoft.AspNetCore.Identity.PasswordVerificationResult.Success;
		}
	}
}
