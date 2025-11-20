using LecturerClaimSystem.Data;
using LecturerClaimSystem.Models;
using LecturerClaimSystem.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace LecturerClaimSystem.Controllers
{
	public class AdminController : Controller
	{
		private readonly ReportService _reports;

		public AdminController(ReportService reports)
		{
			_reports = reports;
		}

		public IActionResult Index(string filter = "all", string? q = null)
		{
			var claims = ClaimDataStore.GetAllClaims();
			ViewBag.Filter = filter;
			ViewBag.Query = q ?? "";

			claims = filter.ToLower() switch
			{
				"pending" => ClaimDataStore.GetClaimsByStatus(ClaimStatus.Pending),
				"verified" => ClaimDataStore.GetClaimsByStatus(ClaimStatus.Verified),
				"approved" => ClaimDataStore.GetClaimsByStatus(ClaimStatus.Approved),
				"declined" => ClaimDataStore.GetClaimsByStatus(ClaimStatus.Declined),
				_ => claims
			};

			if (!string.IsNullOrWhiteSpace(q))
			{
				var term = q.Trim().ToLower();
				claims = claims.Where(c =>
						(!string.IsNullOrWhiteSpace(c.LecturerName) && c.LecturerName.ToLower().Contains(term)) ||
						(!string.IsNullOrWhiteSpace(c.LecturerEmail) && c.LecturerEmail.ToLower().Contains(term)))
					.ToList();
			}

			ViewBag.PendingCount = ClaimDataStore.GetPendingCount();
			ViewBag.VerifiedCount = ClaimDataStore.GetVerifiedCount();
			ViewBag.ApprovedCount = ClaimDataStore.GetApprovedCount();
			ViewBag.DeclinedCount = ClaimDataStore.GetDeclinedCount();

			return View(claims);
		}

		[HttpGet]
		public IActionResult ExportCsv(string filter = "all", string? q = null)
		{
			var claims = ClaimDataStore.GetAllClaims();

			claims = filter.ToLower() switch
			{
				"pending" => ClaimDataStore.GetClaimsByStatus(ClaimStatus.Pending),
				"verified" => ClaimDataStore.GetClaimsByStatus(ClaimStatus.Verified),
				"approved" => ClaimDataStore.GetClaimsByStatus(ClaimStatus.Approved),
				"declined" => ClaimDataStore.GetClaimsByStatus(ClaimStatus.Declined),
				_ => claims
			};

			if (!string.IsNullOrWhiteSpace(q))
			{
				var term = q.Trim().ToLower();
				claims = claims.Where(c =>
						(!string.IsNullOrWhiteSpace(c.LecturerName) && c.LecturerName.ToLower().Contains(term)) ||
						(!string.IsNullOrWhiteSpace(c.LecturerEmail) && c.LecturerEmail.ToLower().Contains(term)))
					.ToList();
			}

			var csv = _reports.ExportClaimsToCsv(claims);
			return File(csv, "text/csv", $"claims_{filter}_{DateTime.Now:yyyyMMddHHmm}.csv");
		}


	}
}