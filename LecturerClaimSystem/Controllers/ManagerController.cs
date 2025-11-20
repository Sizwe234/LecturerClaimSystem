using LecturerClaimSystem.Data;
using LecturerClaimSystem.Helpers;
using LecturerClaimSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace LecturerClaimSystem.Controllers
{
	public class ManagerController : Controller
	{
		private IActionResult Gate()
		{
			if (!HttpContext.Session.IsLoggedIn())
				return RedirectToAction("Login", "Auth", new { returnUrl = Url.Action("Index", "Manager") });
			if (!HttpContext.Session.IsRole(UserRole.Manager.ToString()))
			{
				TempData["Error"] = "Access denied: Manager only.";
				return RedirectToAction("Dashboard", "Lecturer");
			}
			return null!;
		}

		[HttpGet]
		public IActionResult Index()
		{
			var g = Gate(); if (g != null) return g;
			var verified = ClaimDataStore.GetClaimsByStatus(ClaimStatus.Verified);
			return View(verified);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Approve(int id, string? comments)
		{
			var g = Gate(); if (g != null) return g;
			var note = string.IsNullOrWhiteSpace(comments) ? "Approved for payment" : comments!;
			var ok = ClaimDataStore.UpdateClaimStatus(id, ClaimStatus.Approved, "Academic Manager", note);
			TempData[ok ? "Success" : "Error"] = ok ? "Claim approved." : "Claim not found.";
			return RedirectToAction(nameof(Index));
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Decline(int id, string? comments)
		{
			var g = Gate(); if (g != null) return g;
			if (string.IsNullOrWhiteSpace(comments))
			{
				TempData["Error"] = "Please provide a reason for decline.";
				return RedirectToAction(nameof(Index));
			}
			var ok = ClaimDataStore.UpdateClaimStatus(id, ClaimStatus.Declined, "Academic Manager", comments!);
			TempData[ok ? "Success" : "Error"] = ok ? "Claim declined." : "Claim not found.";
			return RedirectToAction(nameof(Index));
		}
	}
}