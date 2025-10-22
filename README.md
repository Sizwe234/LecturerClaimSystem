Lecturer Claim System:

This project is a web app I built to manage lecturer claims. Lecturers can log in, submit their hours, attach supporting documents, and track the status of their claims. Admins can log in on their side, see all claims, verify them, approve or decline them, and keep a full review history. The idea is simple: no more messy spreadsheets or emails, just one clean system.

Features:

[Lecturer's side]
  - Submit a new claim with hours worked, hourly rate, and notes.
 
 - Upload supporting files (PDF, DOCX, XLSX).
 
 - See all your claims and their current status.

[Admin side]
 
 - Dashboard with counts of Pending, Verified, Approved, and Declined.
 
 - Review claims, update status, and leave comments.
 
 - Track review history for accountability.

File handling:

  - Uploads are stored and linked to claims.
 
 - Encryption/decryption service included for secure file handling.

Tests:
  - Add Claim
  - Approve Claim
  - Verify Claim
  - Deny Claim
  - File Encryption
  - File Decryption



Tech Stack:

- ASP.NET Core MVC
- Bootstrap 5 for styling
- In‑memory data store (ClaimDataStore)
- xUnit for tests


 How to Run:

1. Clone the repo.

2. Make sure you have .NET 8 long-term support installed.

3. Run the app
