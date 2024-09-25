
using Api.Database;
using Api.Models;
using Api.Service;
using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Api.Helpers;

namespace Api
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.
			builder.Services.AddControllers();
 
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();

			// Database configuration
			builder.Services.AddDbContext<DatabaseContext>(options =>
				options.UseMySQL(builder.Configuration.GetConnectionString("DefaultConnection")));

			// Authentication and JWT configuration
			builder.Services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			}).AddJwtBearer(options =>
			{
				options.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuer = true,
					ValidateAudience = true,
					ValidateLifetime = true,
					ValidateIssuerSigningKey = true,
					ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
					ValidAudience = builder.Configuration["JwtSettings:Audience"],
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"]))
				};
			});

			// Dependency injection for services
			builder.Services.AddScoped<TokenService>();
			builder.Services.AddScoped<PasswordService>();
			builder.Services.AddScoped<LoginHelper>();
			builder.Services.AddMemoryCache();

			// Configure rate limiting services and other options
			ConfigureServices(builder.Services, builder.Configuration);

			// Configure logging
			builder.Logging.ClearProviders();
			builder.Logging.AddConsole();
			builder.Logging.AddDebug();

			var app = builder.Build();

			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			app.UseHttpsRedirection();

			app.UseAuthentication();
			app.UseAuthorization();

			app.MapControllers();

			app.Run();
		}

		// Configure reate limiting
		public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
		{
			services.AddOptions();
			services.AddMemoryCache();
			services.Configure<IpRateLimitOptions>(configuration.GetSection("IpRateLimiting"));
			services.AddInMemoryRateLimiting();
			services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
		}
	}
}
