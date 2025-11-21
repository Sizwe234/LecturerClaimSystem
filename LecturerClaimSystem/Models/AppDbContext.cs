using LecturerClaimSystem.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LecturerClaimSystem.Data
{
	public class AppDbContext : IdentityDbContext<AppUser>
	{
		public DbSet<Claim> Claims { get; set; } = null!;

		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
		{
		}

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);
		}
	}
}