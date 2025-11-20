using Microsoft.AspNetCore.Mvc;
using LecturerClaimSystem.Models;
using System.Linq;

namespace LecturerClaimSystem.Controllers
{
	public class CoordinatorController : Controller
	{
		private readonly IClaimRepository _repo;

		public CoordinatorController(IClaimRepository repo)
		{
			_repo = repo;
		}

		[HttpGet]
		public IActionResult Index()
		{
			// Show only Pending claims
			var pending = _repo.GetAll().Where(c => c.Status == ClaimStatus.Pending).ToList();
			return View(pending);
		}

		[HttpPost]
		public IActionResult Verify(int id, string? comments)
		{
			var claim = _repo.GetById(id);
			if (claim == null)
			{
				TempData["Error"] = "Claim not found.";
				return RedirectToAction(nameof(Index));
			}

			claim.Status = ClaimStatus.Verified;
			claim.Reviews.Add(new ClaimReview
			{
				ClaimId = claim.Id,
				ReviewerName = "Coordinator",
				ReviewerRole = "Coordinator",
				Decision = ClaimStatus.Verified,
				Comments = comments ?? "",
				ReviewDate = System.DateTime.Now
			});

			_repo.Update(claim);
			TempData["Success"] = "Claim verified.";
			return RedirectToAction(nameof(Index));
		}

		[HttpPost]
		public IActionResult Reject(int id, string? comments)
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
				ReviewerName = "Coordinator",
				ReviewerRole = "Coordinator",
				Decision = ClaimStatus.Declined,
				Comments = comments ?? "",
				ReviewDate = System.DateTime.Now
			});

			_repo.Update(claim);
			TempData["Success"] = "Claim rejected.";
			return RedirectToAction(nameof(Index));
		}
	}
}