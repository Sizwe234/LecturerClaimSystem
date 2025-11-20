using LecturerClaimSystem.Data;
using LecturerClaimSystem.Models;
using LecturerClaimSystem.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;

namespace LecturerClaimSystem.Controllers
{
	public class ClaimsController : Controller
	{
		public IActionResult Index()
		{
			var claims = ClaimDataStore.GetAllClaims();
			return View(claims);
		}

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
		public IActionResult DownloadDocument(int claimId, int documentId)
		{
			var claim = ClaimDataStore.GetClaimById(claimId);
			if (claim == null) { TempData["Error"] = "Claim not found."; return RedirectToAction(nameof(Index)); }

			var doc = claim.Documents.FirstOrDefault(d => d.Id == documentId);
			if (doc == null) { TempData["Error"] = "Document not found."; return RedirectToAction(nameof(Details), new { id = claimId }); }

			var service = HttpContext.RequestServices.GetService(typeof(FileStorageService)) as FileStorageService;
			if (service == null) { TempData["Error"] = "Storage service unavailable."; return RedirectToAction(nameof(Details), new { id = claimId }); }

			var read = service.ReadClaimFile(doc.StoredPath);
			if (!read.ok || read.bytes == null) { TempData["Error"] = read.error ?? "File read error."; return RedirectToAction(nameof(Details), new { id = claimId }); }

			var ext = Path.GetExtension(doc.FileName).ToLowerInvariant();
			var contentType = ext switch
			{
				".pdf" => "application/pdf",
				".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
				".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
				_ => "application/octet-stream"
			};

			return File(read.bytes, contentType, doc.FileName);
		}
	}
}