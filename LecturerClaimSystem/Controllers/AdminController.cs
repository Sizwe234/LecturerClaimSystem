using LecturerClaimSystem.Data;
using LecturerClaimSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace LecturerClaimSystem.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index(string filter = "all")
        {
            var claims = ClaimDataStore.GetAllClaims();
            ViewBag.Filter = filter;

            claims = filter.ToLower() switch
            {
                "pending" => ClaimDataStore.GetClaimsByStatus(ClaimStatus.Pending),
                "verified" => ClaimDataStore.GetClaimsByStatus(ClaimStatus.Verified),
                "approved" => ClaimDataStore.GetClaimsByStatus(ClaimStatus.Approved),
                "declined" => ClaimDataStore.GetClaimsByStatus(ClaimStatus.Declined),
                _ => claims
            };

            ViewBag.PendingCount = ClaimDataStore.GetPendingCount();
            ViewBag.VerifiedCount = ClaimDataStore.GetVerifiedCount();
            ViewBag.ApprovedCount = ClaimDataStore.GetApprovedCount();
            ViewBag.DeclinedCount = ClaimDataStore.GetDeclinedCount();

            return View(claims);
        }

        public IActionResult Review(int id)
        {
            var claim = ClaimDataStore.GetClaimById(id);
            if (claim == null)
            {
                TempData["Error"] = "Claim not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(claim);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Verify(int id, string? comments)
        {
            try
            {
                var note = string.IsNullOrWhiteSpace(comments) ? "Verified by Programme Coordinator" : comments!;
                var success = ClaimDataStore.UpdateClaimStatus(id, ClaimStatus.Verified, "Programme Coordinator", note);
                TempData[success ? "Success" : "Error"] = success ? "Claim verified." : "Claim not found.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Verification failed: {ex.Message}";
                return RedirectToAction(nameof(Review), new { id });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Approve(int id, string? comments)
        {
            try
            {
                var note = string.IsNullOrWhiteSpace(comments) ? "Approved for payment" : comments!;
                var success = ClaimDataStore.UpdateClaimStatus(id, ClaimStatus.Approved, "Academic Manager", note);
                TempData[success ? "Success" : "Error"] = success ? "Claim approved." : "Claim not found.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Approval failed: {ex.Message}";
                return RedirectToAction(nameof(Review), new { id });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Decline(int id, string? comments)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(comments))
                {
                    TempData["Error"] = "Please provide a reason for declining.";
                    return RedirectToAction(nameof(Review), new { id });
                }

                var success = ClaimDataStore.UpdateClaimStatus(id, ClaimStatus.Declined, "Academic Manager", comments!);
                TempData[success ? "Success" : "Error"] = success ? "Claim declined." : "Claim not found.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Decline failed: {ex.Message}";
                return RedirectToAction(nameof(Review), new { id });
            }
        }
    }
}