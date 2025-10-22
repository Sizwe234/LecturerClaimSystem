using Xunit;
using LecturerClaimSystem.Data;
using LecturerClaimSystem.Models;
using LecturerClaimSystem.Services;
using System.Text;

namespace LecturerClaimSystem.Tests
{
    public class ClaimTests
    {
        [Fact]
        public void Test1_AddClaimSuccessful()
        {
            var initialCount = ClaimDataStore.GetAllClaims().Count;

            var newClaim = new Claim
            {
                LecturerName = "Test Lecturer",
                LecturerEmail = "lecturer@test.com",
                HoursWorked = 10,
                HourlyRate = 200,
                Notes = "Test claim"
            };

            ClaimDataStore.AddClaim(newClaim);

            var newCount = ClaimDataStore.GetAllClaims().Count;
            Assert.Equal(initialCount + 1, newCount);

            Assert.True(newClaim.Id > 0, "Claim should have an ID assigned");
            Assert.Equal(ClaimStatus.Pending, newClaim.Status);

            var retrievedClaim = ClaimDataStore.GetClaimById(newClaim.Id);
            Assert.NotNull(retrievedClaim);
            Assert.Equal("Test Lecturer", retrievedClaim!.LecturerName);
        }

        [Fact]
        public async Task Test2_EncryptionFile_Successful()
        {
            var originalContent = "This is a secret file content that should be encrypted";
            var originalBytes = Encoding.UTF8.GetBytes(originalContent);
            var inputStream = new MemoryStream(originalBytes);
            var tempFile = Path.GetTempFileName();
            var encryptionService = new FileEncryptionService();

            try
            {
                await encryptionService.EncryptFileAsync(inputStream, tempFile);

                Assert.True(File.Exists(tempFile), "Encrypted file should exist");

                var encryptedBytes = await File.ReadAllBytesAsync(tempFile);

                Assert.NotEqual(originalBytes, encryptedBytes);
                Assert.True(encryptedBytes.Length > 0, "Encrypted file should have content");

                var encryptedText = Encoding.UTF8.GetString(encryptedBytes);
                Assert.DoesNotContain(originalContent, encryptedText);
            }
            finally
            {
                if (File.Exists(tempFile))
                {
                    File.Delete(tempFile);
                }
            }
        }

        [Fact]
        public async Task Test3_DecryptFile_Successful()
        {
            var originalContent = "This is a secret file content that should be encrypted and then decrypted";
            var inputStream = new MemoryStream(Encoding.UTF8.GetBytes(originalContent));
            var tempFile = Path.GetTempFileName();
            var encryptionService = new FileEncryptionService();

            try
            {
                await encryptionService.EncryptFileAsync(inputStream, tempFile);

                var decryptedStream = await encryptionService.DecryptFileAsync(tempFile);
                var decryptedContent = Encoding.UTF8.GetString(decryptedStream.ToArray());

                Assert.Equal(originalContent, decryptedContent);
                Assert.Contains(originalContent, decryptedContent);
            }
            finally
            {
                if (File.Exists(tempFile))
                {
                    File.Delete(tempFile);
                }
            }
        }

        [Fact]
        public void Test4_ApproveClaim()
        {
            var newClaim = new Claim
            {
                LecturerName = "Lecturer Approve",
                LecturerEmail = "approve@test.com",
                HoursWorked = 5,
                HourlyRate = 150,
                Status = ClaimStatus.Pending
            };

            ClaimDataStore.AddClaim(newClaim);

            var success = ClaimDataStore.UpdateClaimStatus(
                newClaim.Id,
                ClaimStatus.Approved,
                "Admin User",
                "Claim approved for payment"
            );

            Assert.True(success, "Update should succeed");

            var updatedClaim = ClaimDataStore.GetClaimById(newClaim.Id);
            Assert.NotNull(updatedClaim);
            Assert.Equal(ClaimStatus.Approved, updatedClaim!.Status);
            Assert.Equal("Admin User", updatedClaim.ReviewedBy);
        }

        [Fact]
        public void Test5_VerifyClaim()
        {
            var newClaim = new Claim
            {
                LecturerName = "Lecturer Verify",
                LecturerEmail = "verify@test.com",
                HoursWorked = 12,
                HourlyRate = 250,
                Status = ClaimStatus.Pending
            };

            ClaimDataStore.AddClaim(newClaim);

            var success = ClaimDataStore.UpdateClaimStatus(
                newClaim.Id,
                ClaimStatus.Verified,
                "Programme Coordinator",
                "Verified successfully"
            );

            Assert.True(success, "Update should succeed");

            var updatedClaim = ClaimDataStore.GetClaimById(newClaim.Id);
            Assert.NotNull(updatedClaim);
            Assert.Equal(ClaimStatus.Verified, updatedClaim!.Status);
            Assert.Equal("Programme Coordinator", updatedClaim.ReviewedBy);
        }

        [Fact]
        public void Test6_DenyClaim()
        {
            var newClaim = new Claim
            {
                LecturerName = "Lecturer Deny",
                LecturerEmail = "deny@test.com",
                HoursWorked = 8,
                HourlyRate = 100,
                Status = ClaimStatus.Pending
            };

            ClaimDataStore.AddClaim(newClaim);

            var success = ClaimDataStore.UpdateClaimStatus(
                newClaim.Id,
                ClaimStatus.Declined,
                "Admin User",
                "Claim rejected due to incorrect hours"
            );

            Assert.True(success, "Update should succeed");

            var updatedClaim = ClaimDataStore.GetClaimById(newClaim.Id);
            Assert.NotNull(updatedClaim);
            Assert.Equal(ClaimStatus.Declined, updatedClaim!.Status);
            Assert.Equal("Admin User", updatedClaim.ReviewedBy);
            Assert.NotNull(updatedClaim.ReviewedDate);
        }
    }
}
