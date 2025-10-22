using System.Text.RegularExpressions;
using LecturerClaimSystem.Models;

namespace LecturerClaimSystem.Services
{
    public class FileStorageService
    {
        private readonly IWebHostEnvironment _env;
        private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".pdf", ".docx", ".xlsx"
        };
        private const long MaxSizeBytes = 10 * 1024 * 1024; // 10 MB

        public FileStorageService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public (bool ok, string? error, UploadedDocument? doc) SaveClaimFile(IFormFile file, int claimId)
        {
            if (file == null || file.Length == 0)
                return (false, "No file provided.", null);

            if (file.Length > MaxSizeBytes)
                return (false, "File exceeds 10 MB limit.", null);

            var ext = Path.GetExtension(file.FileName);
            if (string.IsNullOrWhiteSpace(ext) || !AllowedExtensions.Contains(ext))
                return (false, "Invalid file type. Allowed: .pdf, .docx, .xlsx", null);

            // Sanitize filename (allow letters, digits, dot, dash, underscore)
            var safeName = Regex.Replace(Path.GetFileName(file.FileName), @"[^A-Za-z0-9\.\-_]", "_");

            var uploadsRoot = Path.Combine(_env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "uploads");
            if (!Directory.Exists(uploadsRoot))
                Directory.CreateDirectory(uploadsRoot);

            var uniqueName = $"{Guid.NewGuid():N}_{safeName}";
            var storedPath = Path.Combine("uploads", uniqueName);
            var fullPath = Path.Combine(uploadsRoot, uniqueName);

            try
            {
                using var stream = new FileStream(fullPath, FileMode.Create);
                file.CopyTo(stream);
            }
            catch (Exception ex)
            {
                return (false, $"Failed to save file: {ex.Message}", null);
            }

            var doc = new UploadedDocument
            {
                ClaimId = claimId,
                FileName = safeName,
                StoredPath = storedPath.Replace('\\', '/'),
                FileSizeBytes = file.Length,
                UploadedAt = DateTime.Now
            };

            return (true, null, doc);
        }
    }
}
