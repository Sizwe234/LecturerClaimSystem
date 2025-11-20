// Helpers/AuthExtensions.cs
using Microsoft.AspNetCore.Http;

namespace LecturerClaimSystem.Helpers
{
	public static class AuthExtensions
	{
		public static string? GetUserRole(this ISession session) =>
			session.GetString("UserRole");

		public static bool IsRole(this ISession session, string role) =>
			session.GetString("UserRole") == role;

		public static bool IsLoggedIn(this ISession session) =>
			!string.IsNullOrWhiteSpace(session.GetString("UserEmail"));
	}
}