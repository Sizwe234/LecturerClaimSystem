using LecturerClaimSystem.Data;
using LecturerClaimSystem.Helpers;
using LecturerClaimSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace LecturerClaimSystem.Controllers
{
	public class CoordinatorController : Controller
	{
		private IActionResult Gate()
		{
			if (!HttpContext.Session.IsLoggedIn())
				return RedirectToAction("Login", "Auth", new { returnUrl = Url.Action("Index", "Coordinator") });
			if (!HttpContext.Session.IsRole(UserRole.Coordinator.ToString()))
			{
				TempData["Error"] = "Access denied: Coordinator only.";
				return RedirectToAction("Dashboard", "Lecturer");
			}
			return null!;
		}

		[HttpGet]
		public IActionResult Index()
		{
			var g = Gate(); if (g != null) return g;
			var pending = ClaimDataStore.GetClaimsByStatus(ClaimStatus.Pending);
			return View(pending);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Verify(int id, string? comments)
		{
			var g = Gate(); if (g != null) return g;
			var note = string.IsNullOrWhiteSpace(comments) ? "Verified by Programme Coordinator" : comments!;
			var ok = ClaimDataStore.UpdateClaimStatus(id, ClaimStatus.Verified, "Programme Coordinator", note);
			TempData[ok ? "Success" : "Error"] = ok ? "Claim verified." : "Claim not found.";
			return RedirectToAction(nameof(Index));
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Reject(int id, string? comments)
		{
			var g = Gate(); if (g != null) return g;
			if (string.IsNullOrWhiteSpace(comments))
			{
				TempData["Error"] = "Please provide a reason for rejection.";
				return RedirectToAction(nameof(Index));
			}
			var ok = ClaimDataStore.UpdateClaimStatus(id, ClaimStatus.Declined, "Programme Coordinator", comments!);
			TempData[ok ? "Success" : "Error"] = ok ? "Claim rejected." : "Claim not found.";
			return RedirectToAction(nameof(Index));
		}
	}
}