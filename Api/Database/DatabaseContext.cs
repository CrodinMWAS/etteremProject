﻿using Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Database
{
	public class DatabaseContext : DbContext
	{
		public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }
		public DbSet<User> Users { get; set; }
		public DbSet<DishModel> Dishes { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<IngredientModel>().HasNoKey();	
			base.OnModelCreating(modelBuilder);
		}
	}
}
