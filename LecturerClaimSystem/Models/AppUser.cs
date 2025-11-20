using System.ComponentModel.DataAnnotations;

namespace LecturerClaimSystem.Models
{
	public class AppUser
	{
		public int Id { get; set; }

		[Required, StringLength(50)]
		public string FirstName { get; set; } = string.Empty;

		[Required, StringLength(50)]
		public string LastName { get; set; } = string.Empty;

		[Required, EmailAddress, StringLength(100)]
		public string Email { get; set; } = string.Empty;

		[Required, StringLength(100)]
		public string Password { get; set; } = string.Empty; // Simple demo; hashed in real apps

		[Range(1, 100000)]
		public decimal HourlyRate { get; set; } = 0;

		[Required]
		public UserRole Role { get; set; } = UserRole.Lecturer;

		public string FullName => $"{FirstName} {LastName}";
	}
}