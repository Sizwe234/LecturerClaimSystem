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

		[Required(ErrorMessage = "Lecturer name is required")]
		[StringLength(100, ErrorMessage = "Name must be 100 characters or fewer")]
		public string LecturerName { get; set; } = string.Empty;

		[Required(ErrorMessage = "Lecturer email is required")]
		[EmailAddress(ErrorMessage = "Please enter a valid email")]
		public string LecturerEmail { get; set; } = string.Empty;

		[Range(1, 1000, ErrorMessage = "Hours must be between 1 and 1000")]
		public int HoursWorked { get; set; }

		[Range(1, 100000, ErrorMessage = "Rate must be between 1 and 100000")]
		public decimal HourlyRate { get; set; }

		[StringLength(1000, ErrorMessage = "Notes must be 1000 characters or fewer")]
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