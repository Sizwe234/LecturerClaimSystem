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

			TempData["Success"] = "User created successfully.";
			return RedirectToAction(nameof(Index));
		}

		[HttpGet]
		public async Task<IActionResult> Edit(string id)
		{
			var user = await _users.FindByIdAsync(id);
			if (user == null) return NotFound();
			return View(user);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(AppUser model, string? password, string role)
		{
			var user = await _users.FindByIdAsync(model.Id);
			if (user == null) return NotFound();

			user.FirstName = model.FirstName;
			user.LastName = model.LastName;
			user.Email = model.Email;
			user.UserName = model.Email;
			user.HourlyRate = model.HourlyRate;

			var updateResult = await _users.UpdateAsync(user);
			if (!updateResult.Succeeded)
			{
				TempData["Error"] = string.Join("; ", updateResult.Errors.Select(e => e.Description));
				return View(model);
			}

			if (!string.IsNullOrWhiteSpace(password))
			{
				var token = await _users.GeneratePasswordResetTokenAsync(user);
				var passResult = await _users.ResetPasswordAsync(user, token, password);
				if (!passResult.Succeeded)
				{
					TempData["Error"] = string.Join("; ", passResult.Errors.Select(e => e.Description));
					return View(model);
				}
			}

			var currentRoles = await _users.GetRolesAsync(user);
			await _users.RemoveFromRolesAsync(user, currentRoles);
			if (!string.IsNullOrWhiteSpace(role))
			{
				await _users.AddToRoleAsync(user, role);
			}

			TempData["Success"] = "User updated successfully.";
			return RedirectToAction(nameof(Index));
		}
	}
}