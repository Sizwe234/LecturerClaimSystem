
using LecturerClaimSystem.Data;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace LecturerClaimSystem.Controllers
{
	public class AuthController : Controller
	{
		private readonly AppDbContext _db;

		public AuthController(AppDbContext db)
		{
			_db = db;
		}

		[HttpGet]
		public IActionResult Login(string? returnUrl = null)
		{
			ViewBag.ReturnUrl = returnUrl;
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Login(string email, string password, string? returnUrl = null)
		{
			var user = _db.Users.FirstOrDefault(u => u.Email.ToLower() == email.ToLower() && u.Password == password);
			if (user == null)
			{
				TempData["Error"] = "Invalid email or password.";
				return View();
			}

			HttpContext.Session.SetString("UserEmail", user.Email);
			HttpContext.Session.SetString("UserName", user.FullName);
			HttpContext.Session.SetString("UserRole", user.Role.ToString());

			TempData["Success"] = $"Welcome, {user.FullName}";
			if (!string.IsNullOrWhiteSpace(returnUrl)) return Redirect(returnUrl);

			return RedirectToAction("Dashboard", "Lecturer");
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Logout()
		{
			HttpContext.Session.Clear();
			TempData["Success"] = "Logged out.";
			return RedirectToAction("Login");
		}
	}
}