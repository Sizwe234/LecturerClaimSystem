using LecturerClaimSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LecturerClaimSystem.Controllers
{
	[Authorize(Roles = "HR")]
	public class HrController : Controller
	{
		private readonly UserManager<AppUser> _userManager;

		public HrController(UserManager<AppUser> userManager)
		{
			_userManager = userManager;
		}

		[HttpGet]
		public IActionResult Index()
		{
			var users = _userManager.Users.ToList();
			return View(users);
		}

		[HttpGet]
		public IActionResult Create() => View();

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(AppUser model, string password, string role)
		{
			if (!ModelState.IsValid) return View(model);

			var result = await _userManager.CreateAsync(model, password);
			if (result.Succeeded)
			{
				if (!string.IsNullOrEmpty(role))
					await _userManager.AddToRoleAsync(model, role);

				TempData["Success"] = "User created successfully.";
				return RedirectToAction(nameof(Index));
			}

			foreach (var error in result.Errors)
				ModelState.AddModelError("", error.Description);

			return View(model);
		}

		[HttpGet]
		public async Task<IActionResult> Edit(string id)
		{
			var user = await _userManager.FindByIdAsync(id);
			if (user == null) return NotFound();
			return View(user);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(AppUser model, string password, string role)
		{
			var user = await _userManager.FindByIdAsync(model.Id);
			if (user == null) return NotFound();

			user.FirstName = model.FirstName;
			user.LastName = model.LastName;
			user.Email = model.Email;
			user.UserName = model.Email;
			user.HourlyRate = model.HourlyRate;

			var result = await _userManager.UpdateAsync(user);
			if (!result.Succeeded)
			{
				foreach (var error in result.Errors)
					ModelState.AddModelError("", error.Description);
				return View(model);
			}

			if (!string.IsNullOrEmpty(password))
			{
				var token = await _userManager.GeneratePasswordResetTokenAsync(user);
				await _userManager.ResetPasswordAsync(user, token, password);
			}

			var currentRoles = await _userManager.GetRolesAsync(user);
			await _userManager.RemoveFromRolesAsync(user, currentRoles);
			if (!string.IsNullOrEmpty(role))
				await _userManager.AddToRoleAsync(user, role);

			TempData["Success"] = "User updated successfully.";
			return RedirectToAction(nameof(Index));
		}


		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Delete(string id)
		{
			var user = await _userManager.FindByIdAsync(id);
			if (user == null) return NotFound();

			var result = await _userManager.DeleteAsync(user);
			if (result.Succeeded)
			{
				TempData["Success"] = "User deleted successfully.";
			}
			else
			{
				TempData["Error"] = "Error deleting user.";
			}

			return RedirectToAction(nameof(Index));
		}
	}
}