using System.Security.Claims;

namespace LecturerClaimSystem.Helpers
{
	public static class AuthExtensions
	{
		public static bool IsHr(this ClaimsPrincipal user) =>
			user.IsInRole("HR");

		public static bool IsLecturer(this ClaimsPrincipal user) =>
			user.IsInRole("Lecturer");

		public static bool IsCoordinator(this ClaimsPrincipal user) =>
			user.IsInRole("Coordinator");

		public static bool IsManager(this ClaimsPrincipal user) =>
			user.IsInRole("Manager");

		public static string? GetEmail(this ClaimsPrincipal user) =>
			user.FindFirstValue(ClaimTypes.Email);

		public static string? GetName(this ClaimsPrincipal user) =>
			user.Identity?.Name;

		public static bool IsLoggedIn(this ClaimsPrincipal user) =>
			user.Identity?.IsAuthenticated ?? false;
	}
}