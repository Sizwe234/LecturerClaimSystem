namespace LecturerClaimSystem.Models
{
	public class ClaimDocument
	{
		public int Id { get; set; }
		public int ClaimId { get; set; }
		public string FileName { get; set; } = string.Empty;

		
		public string FilePath { get; set; } = string.Empty;

		public string StoredPath
		{
			get => FilePath;
			set => FilePath = value;
		}
	}
}