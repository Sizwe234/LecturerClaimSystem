using LecturerClaimSystem.Data;
using LecturerClaimSystem.Models;
using LecturerClaimSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Linq;

namespace LecturerClaimSystem.Controllers
{
	// Allow HR to submit on behalf of lecturers; lecturers submit for themselves
	[Authorize(Roles = "Lecturer,HR")]
	public class LecturerController : Controller
	{
		private readonly FileStorageService _files;
		private readonly AppDbContext _db;
		private readonly UserManager<AppUser> _userManager;

		public LecturerController(FileStorageService files, AppDbContext db, UserManager<AppUser> userManager)
		{
			_files = files;
			_db = db;
			_userManager = userManager;
		}

		[HttpGet]
		public IActionResult Dashboard(string? email)
		{
			var claims = ClaimDataStore.GetAllClaims();
			if (!string.IsNullOrWhiteSpace(email))
			{
				claims = claims.Where(c => c.LecturerEmail?.ToLower() == email.Trim().ToLower()).ToList();
			}
			return View(claims);
		}

		[HttpGet]
		public IActionResult Details(int id)
		{
			var claim = ClaimDataStore.GetClaimById(id);
			if (claim == null)
			{
				TempData["Error"] = "Claim not found.";
				return RedirectToAction(nameof(Dashboard));
			}
			return View(claim);
		}

		[HttpGet]
		public IActionResult ReviewHistory(int id)
		{
			var claim = ClaimDataStore.GetClaimById(id);
			if (claim == null)
			{
				TempData["Error"] = "Claim not found.";
				return RedirectToAction(nameof(Dashboard));
			}
			return View(claim);
		}

		[HttpGet]
		public async Task<IActionResult> Submit()
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
				return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Submit", "Lecturer") });

			var model = new Claim
			{
				LecturerName = user.FullName,
				LecturerEmail = user.Email!,
				HourlyRate = user.HourlyRate,
				HoursWorked = 0,
				Notes = "",
				Status = ClaimStatus.Pending
			};
			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Submit(Claim model, IFormFile? upload)
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
				return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Submit", "Lecturer") });

			// Trusted fields from Identity
			model.LecturerName = user.FullName;
			model.LecturerEmail = user.Email!;
			model.HourlyRate = user.HourlyRate;

			if (model.HoursWorked < 1 || model.HoursWorked > 180)
				ModelState.AddModelError(nameof(model.HoursWorked), "Hours must be between 1 and 180.");

			if (model.HourlyRate < 1 || model.HourlyRate > 100000)
				ModelState.AddModelError(nameof(model.HourlyRate), "Rate must be between 1 and 100000.");

			// Default status
			model.Status = ClaimStatus.Pending;

			if (!ModelState.IsValid)
			{
				TempData["Error"] = "Please correct the form errors.";
				return View(model);
			}

			// Save claim to in-memory store
			ClaimDataStore.AddClaim(model);

			// Optional file upload
			if (upload != null && upload.Length > 0)
			{
				var extOk = new[] { ".pdf", ".docx", ".xlsx", ".jpg", ".jpeg", ".png" };
				var ext = Path.GetExtension(upload.FileName).ToLowerInvariant();
				var allowed = extOk.Contains(ext) && upload.Length <= 10 * 1024 * 1024;

				if (!allowed)
				{
					TempData["Error"] = "Only .pdf, .docx, .xlsx, .jpg, .jpeg, .png up to 10 MB are allowed.";
					return RedirectToAction(nameof(Details), new { id = model.Id });
				}

				var savedPath = _files.SaveClaimFile(model.Id, upload);
				var doc = new ClaimDocument
				{
					FileName = upload.FileName,
					FilePath = savedPath,
					StoredPath = savedPath,
					ClaimId = model.Id
				};
				ClaimDataStore.AddDocumentToClaim(model.Id, doc);
				TempData["Success"] = "Claim submitted and document uploaded.";
			}
			else
			{
				TempData["Success"] = "Claim submitted.";
			}

			// Redirect: HR → HR Index; Lecturer → Lecturer Dashboard
			var roles = await _userManager.GetRolesAsync(user);
			if (roles.Contains("HR"))
				return RedirectToAction("Index", "Hr");

			return RedirectToAction(nameof(Dashboard), new { email = user.Email });
		}
	}
}