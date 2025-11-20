using System;
using System.Collections.Generic;

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
		public string LecturerName { get; set; } = string.Empty;
		public string LecturerEmail { get; set; } = string.Empty;
		public int HoursWorked { get; set; }
		public decimal HourlyRate { get; set; }
		public string Notes { get; set; } = string.Empty;
		public ClaimStatus Status { get; set; } = ClaimStatus.Pending;
		public DateTime SubmittedDate { get; set; } = DateTime.Now;


		public List<ClaimDocument> Documents { get; set; } = new List<ClaimDocument>();


		public string ReviewedBy { get; set; } = string.Empty;
		public DateTime? ReviewedDate { get; set; }

		public decimal CalculateEarnings()
		{
			return HoursWorked * HourlyRate;
		}

		public List<ClaimReview> Reviews { get; set; } = new List<ClaimReview>();
	}
}