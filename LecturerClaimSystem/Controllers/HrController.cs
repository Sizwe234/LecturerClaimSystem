using LecturerClaimSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LecturerClaimSystem.Controllers
{
	[Authorize(Roles = "HR")]
	public class HrController : Controller
	{
		private readonly UserManager<AppUser> _users;

		public HrController(UserManager<AppUser> users)
		{
			_users = users;
		}

		[HttpGet]
		public IActionResult Index()
		{
			return View(_users.Users.ToList());
		}

		[HttpGet]
		public IActionResult Create()
		{
			return View(new AppUser());
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(AppUser model, string password, string role)
		{
			if (!ModelState.IsValid)
			{
				TempData["Error"] = "Please fix validation errors.";
				return View(model);
			}

			var existing = await _users.FindByEmailAsync(model.Email!);
			if (existing != null)
			{
				TempData["Error"] = "Email already exists.";
				return View(model);
			}

			model.UserName = model.Email!;
			model.EmailConfirmed = true;

			var createResult = await _users.CreateAsync(model, password);
			if (!createResult.Succeeded)
			{
				TempData["Error"] = string.Join("; ", createResult.Errors.Select(e => e.Description));
				return View(model);
			}

			if (!string.IsNullOrWhiteSpace(role))
			{
				await _users.AddToRoleAsync(model, role);
			}

			TempData["Success"] = "User created.";
			return RedirectToAction(nameof(Index));
		}
	}
}