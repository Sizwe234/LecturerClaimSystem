using LecturerClaimSystem.Data;
using LecturerClaimSystem.Models;
using LecturerClaimSystem.Services;
using Microsoft.AspNetCore.Mvc;

namespace LecturerClaimSystem.Controllers
{
    public class ClaimsController : Controller
    {
        private readonly FileStorageService _files;

        public ClaimsController(FileStorageService files)
        {
            _files = files;
        }

        public IActionResult Index()
        {
            var claims = ClaimDataStore.GetAllClaims();
            return View(claims);
        }

        public IActionResult Add() => View(new Claim());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add(Claim claim, IFormFile? upload)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(claim.LecturerName))
                    ModelState.AddModelError(nameof(claim.LecturerName), "Lecturer name is required.");
                if (string.IsNullOrWhiteSpace(claim.LecturerEmail))
                    ModelState.AddModelError(nameof(claim.LecturerEmail), "Lecturer email is required.");
                if (claim.HoursWorked <= 0)
                    ModelState.AddModelError(nameof(claim.HoursWorked), "Hours worked must be greater than 0.");
                if (claim.HourlyRate <= 0)
                    ModelState.AddModelError(nameof(claim.HourlyRate), "Hourly rate must be greater than 0.");

                if (!ModelState.IsValid)
                    return View(claim);

                ClaimDataStore.AddClaim(claim);

                if (upload != null && upload.Length > 0)
                {
                    var result = _files.SaveClaimFile(upload, claim.Id);
                    if (!result.ok)
                    {
                        TempData["Error"] = result.error ?? "File upload failed.";
                    }
                    else
                    {
                        var added = ClaimDataStore.AddDocumentToClaim(claim.Id, result.doc!);
                        if (!added)
                            TempData["Error"] = "File saved but not linked to claim.";
                        else
                            TempData["Success"] = $"Claim submitted. File uploaded: {result.doc!.FileName}";
                    }
                }
                else
                {
                    TempData["Success"] = "Claim submitted successfully.";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Unexpected error: {ex.Message}";
                return View(claim);
            }
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
    }
}