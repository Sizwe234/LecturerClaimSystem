using System.IO;
using Microsoft.AspNetCore.Http;

namespace LecturerClaimSystem.Services
{
	public class FileStorageService
	{
		private readonly string _storagePath;

		public FileStorageService()
		{
			_storagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
			if (!Directory.Exists(_storagePath))
			{
				Directory.CreateDirectory(_storagePath);
			}
		}

		public string SaveClaimFile(int claimId, IFormFile file)
		{
			var claimFolder = Path.Combine(_storagePath, $"claim_{claimId}");
			if (!Directory.Exists(claimFolder))
			{
				Directory.CreateDirectory(claimFolder);
			}

			var filePath = Path.Combine(claimFolder, file.FileName);

			using (var stream = new FileStream(filePath, FileMode.Create))
			{
				file.CopyTo(stream);
			}

			return filePath;
		}

		public byte[] ReadClaimFile(int claimId, string fileName)
		{
			var claimFolder = Path.Combine(_storagePath, $"claim_{claimId}");
			var filePath = Path.Combine(claimFolder, fileName);

			if (!File.Exists(filePath))
			{
				throw new FileNotFoundException("File not found", filePath);
			}

			return File.ReadAllBytes(filePath);
		}
	}
}