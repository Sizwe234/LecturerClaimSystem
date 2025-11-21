using LecturerClaimSystem.Data;
using LecturerClaimSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LecturerClaimSystem.Controllers
{
	[Authorize(Roles = "Coordinator")]
	public class CoordinatorController : Controller
	{
		[HttpGet]
		public IActionResult Index()
		{
			var pending = ClaimDataStore.GetClaimsByStatus(ClaimStatus.Pending);
			return View(pending);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Verify(int id, string? comments)
		{
			var note = string.IsNullOrWhiteSpace(comments) ? "Verified by Programme Coordinator" : comments!;
			var ok = ClaimDataStore.UpdateClaimStatus(id, ClaimStatus.Verified, "Programme Coordinator", note);
			TempData[ok ? "Success" : "Error"] = ok ? "Claim verified." : "Claim not found.";
			return RedirectToAction(nameof(Index));
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Reject(int id, string? comments)
		{
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