using System.Security.Cryptography;
using System.Text;

namespace LecturerClaimSystem.Services
{
    public class FileEncryptionService
    {
        private readonly byte[] _key;
        private readonly byte[] _iv;

        public FileEncryptionService()
        {
            // Simple static key/IV for demo purposes
            // In production, store securely!
            _key = Encoding.UTF8.GetBytes("12345678901234567890123456789012"); // 32 bytes for AES-256
            _iv = Encoding.UTF8.GetBytes("1234567890123456"); // 16 bytes for AES
        }

        public async Task EncryptFileAsync(Stream inputStream, string outputPath)
        {
            using var aes = Aes.Create();
            aes.Key = _key;
            aes.IV = _iv;

            using var cryptoTransform = aes.CreateEncryptor();
            using var outputStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write);
            using var cryptoStream = new CryptoStream(outputStream, cryptoTransform, CryptoStreamMode.Write);

            await inputStream.CopyToAsync(cryptoStream);
        }

        public async Task<MemoryStream> DecryptFileAsync(string inputPath)
        {
            using var aes = Aes.Create();
            aes.Key = _key;
            aes.IV = _iv;

            using var cryptoTransform = aes.CreateDecryptor();
            using var inputStream = new FileStream(inputPath, FileMode.Open, FileAccess.Read);
            using var cryptoStream = new CryptoStream(inputStream, cryptoTransform, CryptoStreamMode.Read);
            var memoryStream = new MemoryStream();
            await cryptoStream.CopyToAsync(memoryStream);
            memoryStream.Position = 0;
            return memoryStream;
        }
    }
}