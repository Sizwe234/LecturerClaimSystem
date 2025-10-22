using System;
using System.Collections.Generic;

namespace LecturerClaimSystem.Models
{
    public class Claim
    {
        public int Id { get; set; }

        public string LecturerName { get; set; } = "";
        public string LecturerEmail { get; set; } = "";
        public decimal HoursWorked { get; set; }
        public decimal HourlyRate { get; set; }
        public string? Notes { get; set; }

        public ClaimStatus Status { get; set; } = ClaimStatus.Pending;
        public DateTime SubmittedDate { get; set; } = DateTime.Now;
        public string? ReviewedBy { get; set; }
        public DateTime? ReviewedDate { get; set; }

        public List<ClaimReview> Reviews { get; set; } = new();
        public List<UploadedDocument> Documents { get; set; } = new();

        public decimal CalculateEarnings() => HoursWorked * HourlyRate;
    }
}