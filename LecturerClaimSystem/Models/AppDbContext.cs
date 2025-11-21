using LecturerClaimSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace LecturerClaimSystem.Data
{
	public class AppDbContext : DbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
		{
		}

		public DbSet<AppUser> Users { get; set; } = null!;
		public DbSet<Claim> Claims { get; set; } = null!;

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{

			modelBuilder.Entity<AppUser>()
						.HasIndex(u => u.Email)
						.IsUnique();


			modelBuilder.Entity<AppUser>().HasData(new AppUser
			{
				Id = 1,
				FirstName = "HR",
				LastName = "Admin",
				Email = "hr@cmcs.local",
				Password = "Pass@123", // demo only
				HourlyRate = 0,
				Role = UserRole.HR
			});
		}
	}
}