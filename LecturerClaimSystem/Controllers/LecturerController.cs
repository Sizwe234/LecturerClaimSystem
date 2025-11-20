using LecturerClaimSystem.Data;
using LecturerClaimSystem.Models;
using LecturerClaimSystem.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace LecturerClaimSystem.Controllers
{
	public class LecturerController : Controller
	{
		private readonly FileStorageService _files;

		public LecturerController(FileStorageService files)
		{
			_files = files;
		}

		public IActionResult Dashboard(string? email)
		{
			var claims = string.IsNullOrWhiteSpace(email)
				? ClaimDataStore.GetAllClaims()
				: ClaimDataStore.GetAllClaims().Where(c => c.LecturerEmail == email).ToList();

			return View(claims);
		}

		public IActionResult Submit()
		{
			return View(new Claim());
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Submit(Claim claim, IFormFile? upload)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(claim.LecturerName))
					ModelState.AddModelError(nameof(claim.LecturerName), "Lecturer name is required.");
				if (string.IsNullOrWhiteSpace(claim.LecturerEmail))
					ModelState.AddModelError(nameof(claim.LecturerEmail), "Lecturer email is required.");
				if (claim.HoursWorked <= 0)
					ModelState.AddModelError(nameof(claim.HoursWorked), "Hours worked must be greater than 0.");
				if (claim.HourlyRate <= 0)
					ModelState.AddModelError(nameof(claim.HourlyRate), "Hourly rate must be greater than 0.");

				if (!ModelState.IsValid)
					return View(claim);

				if (claim.HoursWorked > 10000)
				{
					ModelState.AddModelError(nameof(claim.HoursWorked), "Hours value is not valid.");
					return View(claim);
				}

				ClaimDataStore.AddClaim(claim);

				if (upload != null && upload.Length > 0)
				{
					var result = _files.SaveClaimFile(upload, claim.Id);
					if (!result.ok)
					{
						TempData["Error"] = result.error ?? "File upload failed.";
					}
					else
					{
						var added = ClaimDataStore.AddDocumentToClaim(claim.Id, result.doc!);
						if (!added)
							TempData["Error"] = "File saved but not linked to claim.";
						else
							TempData["Success"] = $"Claim submitted. File uploaded: {result.doc!.FileName}";
					}
				}
				else
				{
					TempData["Success"] = "Claim submitted successfully.";
				}

				return RedirectToAction(nameof(Dashboard), new { email = claim.LecturerEmail });
			}
			catch (Exception ex)
			{
				TempData["Error"] = $"Unexpected error: {ex.Message}";
				return View(claim);
			}
		}

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

		public IActionResult Upload(int id)
		{
			var claim = ClaimDataStore.GetClaimById(id);
			if (claim == null)
			{
				TempData["Error"] = "Claim not found.";
				return RedirectToAction(nameof(Dashboard));
			}
			return View(claim);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Upload(int id, IFormFile? upload)
		{
			var claim = ClaimDataStore.GetClaimById(id);
			if (claim == null)
			{
				TempData["Error"] = "Claim not found.";
				return RedirectToAction(nameof(Dashboard));
			}

			if (upload == null || upload.Length == 0)
			{
				TempData["Error"] = "No file selected.";
				return RedirectToAction(nameof(Details), new { id });
			}

			var result = _files.SaveClaimFile(upload, claim.Id);
			if (!result.ok)
			{
				TempData["Error"] = result.error ?? "File upload failed.";
			}
			else
			{
				var added = ClaimDataStore.AddDocumentToClaim(claim.Id, result.doc!);
				if (!added)
					TempData["Error"] = "File saved but not linked to claim.";
				else
					TempData["Success"] = $"File uploaded: {result.doc!.FileName}";
			}

			return RedirectToAction(nameof(Details), new { id });
		}
	}
}