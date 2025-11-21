
using LecturerClaimSystem.Data;
using LecturerClaimSystem.Helpers;
using LecturerClaimSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace LecturerClaimSystem.Controllers
{
	public class HrController : Controller
	{
		private readonly AppDbContext _db;

		public HrController(AppDbContext db)
		{
			_db = db;
		}

		private IActionResult EnsureHr()
		{
			if (!HttpContext.Session.IsLoggedIn())
				return RedirectToAction("Login", "Auth", new { returnUrl = Url.Action("Index", "Hr") });
			if (!HttpContext.Session.IsRole(UserRole.HR.ToString()))
			{
				TempData["Error"] = "Access denied: HR only.";
				return RedirectToAction("Dashboard", "Lecturer");
			}
			return null!;
		}

		[HttpGet]
		public IActionResult Index()
		{
			var gate = EnsureHr(); if (gate != null) return gate;
			var users = _db.Users.OrderBy(u => u.Role).ThenBy(u => u.LastName).ToList();
			return View(users);
		}

		[HttpGet]
		public IActionResult Create()
		{
			var gate = EnsureHr(); if (gate != null) return gate;
			return View(new AppUser { Role = UserRole.Lecturer, HourlyRate = 0 });
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Create(AppUser model)
		{
			var gate = EnsureHr(); if (gate != null) return gate;

			if (!ModelState.IsValid)
			{
				TempData["Error"] = "Please fix validation errors.";
				return View(model);
			}

			if (_db.Users.Any(u => u.Email.ToLower() == model.Email.ToLower()))
			{
				TempData["Error"] = "Email already exists.";
				return View(model);
			}

			_db.Users.Add(model);
			_db.SaveChanges();
			TempData["Success"] = "User created.";
			return RedirectToAction(nameof(Index));
		}

		[HttpGet]
		public IActionResult Edit(int id)
		{
			var gate = EnsureHr(); if (gate != null) return gate;
			var user = _db.Users.Find(id);
			if (user == null)
			{
				TempData["Error"] = "User not found.";
				return RedirectToAction(nameof(Index));
			}
			return View(user);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Edit(AppUser model)
		{
			var gate = EnsureHr(); if (gate != null) return gate;

			if (!ModelState.IsValid)
			{
				TempData["Error"] = "Please fix validation errors.";
				return View(model);
			}

			var existing = _db.Users.Find(model.Id);
			if (existing == null)
			{
				TempData["Error"] = "User not found.";
				return RedirectToAction(nameof(Index));
			}

			existing.FirstName = model.FirstName;
			existing.LastName = model.LastName;
			existing.Email = model.Email;
			existing.Password = model.Password;
			existing.HourlyRate = model.HourlyRate;
			existing.Role = model.Role;

			_db.SaveChanges();
			TempData["Success"] = "User updated.";
			return RedirectToAction(nameof(Index));
		}
	}
}