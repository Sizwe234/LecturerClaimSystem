using LecturerClaimSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LecturerClaimSystem.Controllers
{
	public class AuthController : Controller
	{
		private readonly SignInManager<AppUser> _signIn;
		private readonly UserManager<AppUser> _users;

		public AuthController(SignInManager<AppUser> signIn, UserManager<AppUser> users)
		{
			_signIn = signIn;
			_users = users;
		}

		[HttpGet]
		public IActionResult Login(string? returnUrl = null)
		{
			ViewBag.ReturnUrl = returnUrl;
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Login(string email, string password, string? returnUrl = null)
		{
			var user = await _users.FindByEmailAsync(email);
			if (user == null)
			{
				TempData["Error"] = "Invalid email or password.";
				return View();
			}

			var result = await _signIn.PasswordSignInAsync(user, password, false, false);
			if (!result.Succeeded)
			{
				TempData["Error"] = "Invalid email or password.";
				return View();
			}

			TempData["Success"] = $"Welcome, {user.FullName}";
			if (!string.IsNullOrWhiteSpace(returnUrl)) return Redirect(returnUrl);

			var roles = await _users.GetRolesAsync(user);
			if (roles.Contains("HR")) return RedirectToAction("Index", "Hr");
			if (roles.Contains("Coordinator")) return RedirectToAction("Index", "Coordinator");
			if (roles.Contains("Manager")) return RedirectToAction("Index", "Manager");
			return RedirectToAction("Dashboard", "Lecturer");
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Logout()
		{
			await _signIn.SignOutAsync();
			TempData["Success"] = "Logged out.";
			return RedirectToAction("Login");
		}
	}
}