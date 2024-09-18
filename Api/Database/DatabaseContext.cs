using Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Database
{
	public class DatabaseContext : DbContext
	{
		public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }
		public DbSet<User> Users { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<User>(entity =>
			{
				entity.ToTable("users");

				entity.HasKey(x => x.Id);
				entity.Property(x => x.Username);
				entity.Property(x => x.Email);
				entity.Property(x => x.PasswordHash);
			});
		}
	}
}
