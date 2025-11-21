using LecturerClaimSystem.Models;
using LecturerClaimSystem.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LecturerClaimSystem.Controllers
{
	public class AccountController : Controller
	{
		private readonly SignInManager<AppUser> _signInManager;
		private readonly UserManager<AppUser> _userManager;

		public AccountController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager)
		{
			_signInManager = signInManager;
			_userManager = userManager;
		}

		[HttpGet]
		public IActionResult Login()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Login(LoginViewModel model)
		{
			if (!ModelState.IsValid) return View(model);

			var result = await _signInManager.PasswordSignInAsync(
				model.Email, model.Password, model.RememberMe, false);

			if (result.Succeeded)
			{
				var user = await _userManager.FindByEmailAsync(model.Email);
				if (user == null)
				{
					ModelState.AddModelError("", "User not found.");
					return View(model);
				}

				var roles = await _userManager.GetRolesAsync(user);

				if (roles.Contains("HR"))
					return RedirectToAction("Index", "Hr");
				else if (roles.Contains("Lecturer"))
					return RedirectToAction("Submit", "Lecturer");
				else if (roles.Contains("Coordinator"))
					return RedirectToAction("Index", "Coordinator");

				return RedirectToAction("Index", "Home");
			}

			ModelState.AddModelError("", "Invalid login attempt.");
			return View(model);
		}

		public async Task<IActionResult> Logout()
		{
			await _signInManager.SignOutAsync();
			return RedirectToAction("Login", "Account");
		}
	}
}