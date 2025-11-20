using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LecturerClaimSystem.Models
{
	public enum ClaimStatus
	{
		Pending,
		Verified,
		Approved,
		Declined
	}

	public class Claim
	{
		public int Id { get; set; }

		[Required]
		public string LecturerName { get; set; } = "";

		[Required, EmailAddress]
		public string LecturerEmail { get; set; } = "";

		public decimal HoursWorked { get; set; }
		public decimal HourlyRate { get; set; }
		public string Notes { get; set; } = "";

		public ClaimStatus Status { get; set; } = ClaimStatus.Pending;
		public DateTime SubmittedDate { get; set; } = DateTime.Now;

		public string? ReviewedBy { get; set; }
		public DateTime? ReviewedDate { get; set; }

		public List<UploadedDocument> Documents { get; set; } = new();
		public List<ClaimReview> Reviews { get; set; } = new();

		public decimal CalculateEarnings() => HoursWorked * HourlyRate;
	}
}