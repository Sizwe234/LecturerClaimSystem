using LecturerClaimSystem.Data;
using LecturerClaimSystem.Helpers;
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
		private readonly AppDbContext _db;

		public LecturerController(FileStorageService files, AppDbContext db)
		{
			_files = files;
			_db = db;
		}

		private AppUser? CurrentLecturer()
		{
			var role = HttpContext.Session.GetString("UserRole");
			var email = HttpContext.Session.GetString("UserEmail");
			if (string.IsNullOrWhiteSpace(email) || role != UserRole.Lecturer.ToString()) return null;
			return _db.Users.FirstOrDefault(u => u.Email.ToLower() == email.ToLower() && u.Role == UserRole.Lecturer);
		}

		[HttpGet]
		public IActionResult Dashboard(string? email)
		{
			if (!HttpContext.Session.IsLoggedIn())
				return RedirectToAction("Login", "Auth", new { returnUrl = Url.Action("Dashboard", "Lecturer") });

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
			if (!HttpContext.Session.IsLoggedIn())
				return RedirectToAction("Login", "Auth", new { returnUrl = Url.Action("Details", "Lecturer", new { id }) });

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
			var lecturer = CurrentLecturer();
			if (lecturer == null)
				return RedirectToAction("Login", "Auth", new { returnUrl = Url.Action("Submit", "Lecturer") });

			// Always pass a Claim model to avoid NullReference
			var model = new Claim
			{
				LecturerName = lecturer.FullName,
				LecturerEmail = lecturer.Email,
				HourlyRate = lecturer.HourlyRate,
				HoursWorked = 0,
				Notes = ""
			};
			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Submit(Claim model, IFormFile? upload)
		{
			var lecturer = CurrentLecturer();
			if (lecturer == null)
				return RedirectToAction("Login", "Auth", new { returnUrl = Url.Action("Submit", "Lecturer") });

			// Force trusted fields from HR data
			model.LecturerName = lecturer.FullName;
			model.LecturerEmail = lecturer.Email;
			model.HourlyRate = lecturer.HourlyRate;

			if (model.HoursWorked > 180)
			{
				ModelState.AddModelError(nameof(model.HoursWorked), "Hours cannot exceed 180 in a month.");
			}

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

			return RedirectToAction(nameof(Dashboard), new { email = lecturer.Email });
		}

		[HttpGet]
		public IActionResult Upload(int id)
		{
			var lecturer = CurrentLecturer();
			if (lecturer == null)
				return RedirectToAction("Login", "Auth", new { returnUrl = Url.Action("Upload", "Lecturer", new { id }) });

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
			var lecturer = CurrentLecturer();
			if (lecturer == null)
				return RedirectToAction("Login", "Auth", new { returnUrl = Url.Action("Upload", "Lecturer", new { id }) });

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