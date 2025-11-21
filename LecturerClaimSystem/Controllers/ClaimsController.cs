using LecturerClaimSystem.Data;
using LecturerClaimSystem.Models;
using LecturerClaimSystem.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using LecturerClaimSystem.Helpers;
using System.IO;
using System.Linq;

namespace LecturerClaimSystem.Controllers
{
	public class ClaimsController : Controller
	{
		private readonly FileStorageService _files;

		public ClaimsController(FileStorageService files)
		{
			_files = files;
		}

		[HttpGet]
		public IActionResult Index()
		{
			var claims = ClaimDataStore.GetAllClaims();
			return View(claims);
		}

		[HttpGet]
		public IActionResult Details(int id)
		{
			var claim = ClaimDataStore.GetClaimById(id);
			if (claim == null)
			{
				TempData["Error"] = "Claim not found.";
				return RedirectToAction(nameof(Index));
			}
			return View(claim);
		}

		[HttpGet]
		public IActionResult Add()
		{
			return View(new Claim());
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Add(Claim model, IFormFile? upload)
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
				TempData["Success"] = "Claim created and document uploaded.";
			}
			else
			{
				TempData["Success"] = "Claim created.";
			}

			return RedirectToAction(nameof(Index));
		}

		[HttpGet]
		public IActionResult DownloadDocument(int claimId, int documentId)
		{
			var claim = ClaimDataStore.GetClaimById(claimId);
			if (claim == null)
			{
				TempData["Error"] = "Claim not found.";
				return RedirectToAction(nameof(Index));
			}

			var doc = claim.Documents.FirstOrDefault(d => d.Id == documentId);
			if (doc == null)
			{
				TempData["Error"] = "Document not found.";
				return RedirectToAction(nameof(Details), new { id = claimId });
			}

			byte[] fileBytes;
			try
			{
				fileBytes = _files.ReadClaimFile(claimId, doc.FileName);
			}
			catch (FileNotFoundException)
			{
				TempData["Error"] = "File not found on server.";
				return RedirectToAction(nameof(Details), new { id = claimId });
			}

			var ext = Path.GetExtension(doc.FileName).ToLowerInvariant();
			var contentType = ext switch
			{
				".pdf" => "application/pdf",
				".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
				".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
				_ => "application/octet-stream"
			};

			return File(fileBytes, contentType, doc.FileName);
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