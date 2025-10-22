using System;

namespace LecturerClaimSystem.Models
{
    public class UploadedDocument
    {
        public int Id { get; set; }
        public int ClaimId { get; set; }
        public string FileName { get; set; } = "";
        public string StoredPath { get; set; } = ""; // relative path under wwwroot
        public long FileSizeBytes { get; set; }
        public DateTime UploadedAt { get; set; } = DateTime.Now;
    }
}