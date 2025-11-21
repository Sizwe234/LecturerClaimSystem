using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace LecturerClaimSystem.Models
{
	public class AppUser : IdentityUser
	{
		[Required, StringLength(50)]
		public string FirstName { get; set; } = string.Empty;

		[Required, StringLength(50)]
		public string LastName { get; set; } = string.Empty;

		[Range(1, 100000)]
		public decimal HourlyRate { get; set; } = 0;

		public string FullName => $"{FirstName} {LastName}";
	}
}