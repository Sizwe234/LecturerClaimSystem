using LecturerClaimSystem.Data;
using LecturerClaimSystem.Models;
using LecturerClaimSystem.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
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

		[HttpGet]
		public IActionResult Dashboard(string? email)
		{
			var claims = ClaimDataStore.GetAllClaims();

			if (!string.IsNullOrWhiteSpace(email))
			{
				claims = claims
					.Where(c => !string.IsNullOrWhiteSpace(c.LecturerEmail) &&
								c.LecturerEmail.Trim().ToLower() == email.Trim().ToLower())
					.ToList();
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
		public IActionResult Submit()
		{
			return View(new Claim());
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Submit(Claim model, IFormFile? upload)
		{
			if (!ModelState.IsValid)
			{
				TempData["Error"] = "Please correct the form errors.";
				return View(model);
			}

			ClaimDataStore.AddClaim(model);

			if (upload != null && upload.Length > 0)
			{
				if (!IsAllowedFile(upload))
				{
					TempData["Error"] = "Only .pdf, .docx, .xlsx up to 10 MB are allowed.";
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

			return RedirectToAction(nameof(Dashboard));
		}

		[HttpGet]
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
				TempData["Error"] = "Please select a file to upload.";
				return RedirectToAction(nameof(Upload), new { id });
			}

			if (!IsAllowedFile(upload))
			{
				TempData["Error"] = "Only .pdf, .docx, .xlsx up to 10 MB are allowed.";
				return RedirectToAction(nameof(Upload), new { id });
			}

			var savedPath = _files.SaveClaimFile(id, upload);

			var doc = new ClaimDocument
			{
				FileName = upload.FileName,
				FilePath = savedPath,
				StoredPath = savedPath,
				ClaimId = id
			};

			ClaimDataStore.AddDocumentToClaim(id, doc);
			TempData["Success"] = "Document uploaded.";
			return RedirectToAction(nameof(Details), new { id });
		}

		private bool IsAllowedFile(IFormFile file)
		{
			var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
			var allowed = ext == ".pdf" || ext == ".docx" || ext == ".xlsx";
			var underLimit = file.Length <= 10 * 1024 * 1024;
			return allowed && underLimit;
		}
	}
}