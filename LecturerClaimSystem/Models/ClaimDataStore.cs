using System;
using System.Collections.Generic;
using System.Linq;
using LecturerClaimSystem.Models;

namespace LecturerClaimSystem.Data
{
    public static class ClaimDataStore
    {
        private static readonly List<Claim> _claims = new();
        private static int _nextId = 1;

        public static List<Claim> GetAllClaims() => _claims;

        public static Claim? GetClaimById(int id) => _claims.FirstOrDefault(c => c.Id == id);

        public static void AddClaim(Claim claim)
        {
            claim.Id = _nextId++;
            claim.SubmittedDate = DateTime.Now;
            claim.Status = ClaimStatus.Pending;
            _claims.Add(claim);
        }

        public static bool AddDocumentToClaim(int claimId, UploadedDocument doc)
        {
            var claim = GetClaimById(claimId);
            if (claim == null) return false;
            doc.Id = (claim.Documents.Count == 0 ? 1 : claim.Documents.Max(d => d.Id) + 1);
            claim.Documents.Add(doc);
            return true;
        }

        public static List<Claim> GetClaimsByStatus(ClaimStatus status) =>
            _claims.Where(c => c.Status == status).ToList();

        public static int GetPendingCount() => _claims.Count(c => c.Status == ClaimStatus.Pending);
        public static int GetVerifiedCount() => _claims.Count(c => c.Status == ClaimStatus.Verified);
        public static int GetApprovedCount() => _claims.Count(c => c.Status == ClaimStatus.Approved);
        public static int GetDeclinedCount() => _claims.Count(c => c.Status == ClaimStatus.Declined);

        public static bool UpdateClaimStatus(int id, ClaimStatus status, string reviewedBy, string comments)
        {
            var claim = GetClaimById(id);
            if (claim == null) return false;

            claim.Status = status;
            claim.ReviewedBy = reviewedBy;
            claim.ReviewedDate = DateTime.Now;

            claim.Reviews.Add(new ClaimReview
            {
                Id = claim.Reviews.Count + 1,
                ClaimId = claim.Id,
                ReviewerName = reviewedBy,
                ReviewerRole = reviewedBy.Contains("Coordinator", StringComparison.OrdinalIgnoreCase)
                                 ? "Programme Coordinator" : "Academic Manager",
                ReviewDate = DateTime.Now,
                Decision = status,
                Comments = comments
            });

            return true;
        }
    }
}