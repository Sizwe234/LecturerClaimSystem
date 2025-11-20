using System;

namespace LecturerClaimSystem.Models
{

	public class UploadedDocument : ClaimDocument
	{

		public long FileSizeBytes { get; set; }
		public DateTime UploadedAt { get; set; } = DateTime.Now;
	}
}