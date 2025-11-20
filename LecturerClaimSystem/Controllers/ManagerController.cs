using LecturerClaimSystem.Data;
using LecturerClaimSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace LecturerClaimSystem.Controllers
{
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
			var success = ClaimDataStore.UpdateClaimStatus(id, ClaimStatus.Approved, "Academic Manager", note);

			TempData[success ? "Success" : "Error"] = success ? "Claim approved." : "Claim not found.";
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

			var success = ClaimDataStore.UpdateClaimStatus(id, ClaimStatus.Declined, "Academic Manager", comments!);
			TempData[success ? "Success" : "Error"] = success ? "Claim declined." : "Claim not found.";
			return RedirectToAction(nameof(Index));
		}
	}
}