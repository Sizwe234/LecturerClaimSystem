using LecturerClaimSystem.Data;
using LecturerClaimSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LecturerClaimSystem.Controllers
{
	[Authorize(Roles = "Manager")]
	public class ManagerController : Controller
	{
		[HttpGet]
		public IActionResult Index()
		{
			var verified = ClaimDataStore.GetClaimsByStatus(ClaimStatus.Verified);
			return View(verified);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Approve(int id, string? comments)
		{
			var note = string.IsNullOrWhiteSpace(comments) ? "Approved for payment" : comments!;
			var ok = ClaimDataStore.UpdateClaimStatus(id, ClaimStatus.Approved, "Academic Manager", note);
			TempData[ok ? "Success" : "Error"] = ok ? "Claim approved." : "Claim not found.";
			return RedirectToAction(nameof(Index));
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Decline(int id, string? comments)
		{
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