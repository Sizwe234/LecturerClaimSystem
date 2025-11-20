using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using LecturerClaimSystem.Models;

namespace LecturerClaimSystem.Services
{
	public class FileStorageService
	{
		private readonly string[] _permittedExtensions = { ".pdf", ".docx", ".xlsx" };
		private readonly long _maxFileBytes = 10 * 1024 * 1024; // 10 MB
		private readonly string _storageRoot;
		private readonly byte[] _aesKey; // 32 bytes

		public FileStorageService(IConfiguration configuration)
		{
			_storageRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "secureuploads");
			Directory.CreateDirectory(_storageRoot);

			var base64Key = configuration["FileEncryptionKey"];
			if (string.IsNullOrWhiteSpace(base64Key))
			{
				using var rng = RandomNumberGenerator.Create();
				_aesKey = new byte[32];
				rng.GetBytes(_aesKey);
			}
			else
			{
				_aesKey = Convert.FromBase64String(base64Key);
			}
		}

		public (bool ok, UploadedDocument? doc, string? error) SaveClaimFile(IFormFile file, int claimId)
		{
			try
			{
				if (file == null || file.Length == 0)
					return (false, null, "File was empty.");

				if (file.Length > _maxFileBytes)
					return (false, null, "File exceeds the maximum allowed size of 10 MB.");

				var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
				if (string.IsNullOrWhiteSpace(ext) || !_permittedExtensions.Contains(ext))
					return (false, null, "Invalid file type. Only PDF, DOCX, XLSX are allowed.");

				var safeName = $"{claimId}_{DateTime.UtcNow:yyyyMMddHHmmss}_{Guid.NewGuid()}{ext}";
				var savedPath = Path.Combine(_storageRoot, safeName);

				using var ms = new MemoryStream();
				file.CopyTo(ms);
				var plainBytes = ms.ToArray();
				var encrypted = EncryptBytes(plainBytes);

				File.WriteAllBytes(savedPath, encrypted);

				var relative = Path.Combine("secureuploads", safeName).Replace("\\", "/");

				var doc = new UploadedDocument
				{
					Id = 0,
					ClaimId = claimId,
					FileName = Path.GetFileName(file.FileName),
					StoredPath = relative,
					FileSizeBytes = file.Length,
					UploadedAt = DateTime.Now
				};

				return (true, doc, null);
			}
			catch (Exception ex)
			{
				return (false, null, $"File save error: {ex.Message}");
			}
		}

		public (bool ok, byte[]? bytes, string? error) ReadClaimFile(string storedRelativePath)
		{
			try
			{
				var full = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", storedRelativePath.Replace("/", Path.DirectorySeparatorChar.ToString()));
				if (!File.Exists(full))
					return (false, null, "File not found.");

				var encrypted = File.ReadAllBytes(full);
				var decrypted = DecryptBytes(encrypted);
				return (true, decrypted, null);
			}
			catch (Exception ex)
			{
				return (false, null, $"Read error: {ex.Message}");
			}
		}

		private byte[] EncryptBytes(byte[] plain)
		{
			using var aes = Aes.Create();
			aes.Key = _aesKey;
			aes.Mode = CipherMode.CBC;
			aes.Padding = PaddingMode.PKCS7;
			aes.GenerateIV();
			using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
			using var ms = new MemoryStream();
			ms.Write(aes.IV, 0, aes.IV.Length);
			using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
			{
				cs.Write(plain, 0, plain.Length);
			}
			return ms.ToArray();
		}

		private byte[] DecryptBytes(byte[] encrypted)
		{
			using var aes = Aes.Create();
			aes.Key = _aesKey;
			aes.Mode = CipherMode.CBC;
			aes.Padding = PaddingMode.PKCS7;

			var iv = encrypted.Take(aes.BlockSize / 8).ToArray();
			var cipher = encrypted.Skip(iv.Length).ToArray();

			using var decryptor = aes.CreateDecryptor(aes.Key, iv);
			using var ms = new MemoryStream();
			using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Write))
			{
				cs.Write(cipher, 0, cipher.Length);
			}
			return ms.ToArray();
		}
	}
}