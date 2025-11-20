using Microsoft.AspNetCore.Mvc;
using LecturerClaimSystem.Models;
using System.Linq;

namespace LecturerClaimSystem.Controllers
{
	public class ManagerController : Controller
	{
		private readonly IClaimRepository _repo;

		public ManagerController(IClaimRepository repo)
		{
			_repo = repo;
		}

		[HttpGet]
		public IActionResult Index()
		{
			// Show only Verified claims
			var verified = _repo.GetAll().Where(c => c.Status == ClaimStatus.Verified).ToList();
			return View(verified);
		}

		[HttpPost]
		public IActionResult Approve(int id, string? comments)
		{
			var claim = _repo.GetById(id);
			if (claim == null)
			{
				TempData["Error"] = "Claim not found.";
				return RedirectToAction(nameof(Index));
			}

			claim.Status = ClaimStatus.Approved;
			claim.Reviews.Add(new ClaimReview
			{
				ClaimId = claim.Id,
				ReviewerName = "Manager",
				ReviewerRole = "Manager",
				Decision = ClaimStatus.Approved,
				Comments = comments ?? "",
				ReviewDate = System.DateTime.Now
			});

			_repo.Update(claim);
			TempData["Success"] = "Claim approved.";
			return RedirectToAction(nameof(Index));
		}

		[HttpPost]
		public IActionResult Decline(int id, string? comments)
		{
			var claim = _repo.GetById(id);
			if (claim == null)
			{
				TempData["Error"] = "Claim not found.";
				return RedirectToAction(nameof(Index));
			}

			claim.Status = ClaimStatus.Declined;
			claim.Reviews.Add(new ClaimReview
			{
				ClaimId = claim.Id,
				ReviewerName = "Manager",
				ReviewerRole = "Manager",
				Decision = ClaimStatus.Declined,
				Comments = comments ?? "",
				ReviewDate = System.DateTime.Now
			});

			_repo.Update(claim);
			TempData["Success"] = "Claim declined.";
			return RedirectToAction(nameof(Index));
		}
	}
}