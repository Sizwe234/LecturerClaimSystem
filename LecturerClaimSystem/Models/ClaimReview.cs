using System;

namespace LecturerClaimSystem.Models
{
	public class ClaimReview
	{
		public int Id { get; set; }
		public int ClaimId { get; set; }

		public DateTime ReviewDate { get; set; } = DateTime.Now;
		public string ReviewerName { get; set; } = "";
		public string ReviewerRole { get; set; } = "";


		public ClaimStatus Decision { get; set; }

		public string Comments { get; set; } = "";
	}
}