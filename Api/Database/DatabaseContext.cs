using Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Database
{
	public class DatabaseContext : DbContext
	{
		public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }
		public DbSet<User> Users { get; set; }
		public DbSet<Dish> Dishes { get; set; }
		public DbSet<Order> Orders { get; set; }
		public DbSet<Stock> Stock { get; set; }
		public DbSet<Ingredient> Ingredients { get; set; }
	
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Ingredient>().HasKey(i => new { i.DishId, i.StockId });
			modelBuilder.Entity<Order>()
				.HasOne(o => o.User)
				.WithMany(u => u.Orders)
				.HasForeignKey(o => o.UserId);

			modelBuilder.Entity<Order>()
				.HasOne(o => o.Dish)
				.WithMany(d => d.Orders)
				.HasForeignKey(o => o.DishId);

			modelBuilder.Entity<Ingredient>()
				.HasOne(i => i.Dish)
				.WithMany(d => d.Ingredients)
				.HasForeignKey(i => i.DishId);

			modelBuilder.Entity<Ingredient>()
				.HasOne(i => i.Stock)
				.WithMany(s => s.Ingredients)
				.HasForeignKey(i => i.StockId);

			base.OnModelCreating(modelBuilder);
		}


	}
}
