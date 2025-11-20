using Xunit;
using LecturerClaimSystem.Models;
using System;

namespace LecturerClaimSystem.Tests
{
	public class ClaimTests
	{
		[Fact]
		public void CalculateEarnings_ReturnsCorrectValue()
		{
			var claim = new Claim { HoursWorked = 10, HourlyRate = 200 };
			Assert.Equal(2000, claim.CalculateEarnings());
		}

		[Fact]
		public void StatusTransition_VerifyThenApprove()
		{
			var claim = new Claim { Id = 1, Status = ClaimStatus.Pending };
			claim.Status = ClaimStatus.Verified;
			Assert.Equal(ClaimStatus.Verified, claim.Status);

			claim.Status = ClaimStatus.Approved;
			Assert.Equal(ClaimStatus.Approved, claim.Status);
		}
	}
}