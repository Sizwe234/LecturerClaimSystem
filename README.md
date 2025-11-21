
Lecturer Claim System

This project is a web app I built to manage lecturer claims. Lecturers can log in, submit their hours, attach supporting documents, and track the status of their claims. Coordinators and Managers can log in on their side, review claims, verify or approve them, and keep a full review history. HR has their own dashboard to register, edit, and delete users for the different functions. The idea is simple: no more messy spreadsheets or emails, just one clean system.



Features

[Lecturer’s side]
- Submit a new claim with hours worked, hourly rate, and notes.  
- Upload supporting files (PDF, DOCX, XLSX, JPG, PNG).  
- See all your claims and their current status.  
- View full review history for accountability.  

[Coordinator’s side]
- Dashboard showing pending lecturer claims.  
- Verify or reject claims, with comments.  

[Manager’s side]
- Dashboard showing verified claims.  
- Approve or decline claims, with comments.  

[HR side]
- Dedicated HR Dashboard.  
- Register new users (Lecturer, Coordinator, Manager, HR).  
- Edit user details (name, email, password, role, hourly rate).  
- Delete users when no longer needed.  
- Emergency ability to log claims on behalf of lecturers.  


File Handling
- Uploads are stored and linked to claims.  
- Encryption/decryption service included for secure file handling.  



Tests
- Add Claim  
- Approve Claim  
- Verify Claim  
- Deny Claim  
- File Encryption  
- File Decryption  
- User Registration  
- User Deletion  


Tech Stack
- ASP.NET Core MVC  
- Bootstrap 5 for styling  
- Identity for authentication and role management  
- SQLite database (via EF Core)  
- xUnit for tests  


How to Run

1. Clone the repo.  
2. Make sure you have **.NET 8 long‑term support** installed.  
3. Run the app with:
   ```bash
   dotnet run
   ```
4. The app will launch at `https://localhost:5001` (or your configured port).  


Login Details

On first run, the system seeds a default HR account:

- Email: hr@cms.local  
- Password: Pass@123  
- Role: HR only  

This account is critical — it is the first place you must log in. When you log in with these credentials, you will be taken directly to the **HR Dashboard**. From here, HR can register new users for the different functions. Without this step, no other users can be added, and the app cannot be tested or used.


Adding New Users

Once logged in as HR:

1. Navigate to the **HR Dashboard**.  
2. Click **Create New User**.  
3. Fill in the form:
   - First name  
   - Last name  
   - Email (this will be the login username)  
   - Password  
   - Role (Lecturer, Coordinator, Manager, HR)  
   - Hourly rate (for lecturers)  
4. Submit the form.  

The new user is now registered in the system. They can log in via `/Account/Login` using the email and password you assigned. Based on their role, they will be redirected to the correct dashboard.


Managing Users

On the HR Dashboard, HR can:
- **Edit** user details (name, email, password, role, hourly rate).  
- **Delete** users if they are no longer needed.  

This gives HR full control over the user lifecycle.


Workflow Summary:

- HR logs in first (`hr@cms.local` → HR Dashboard).  
- HR registers lecturers, coordinators, and managers.  
- Lecturers log in and submit claims.  
- Coordinators verify claims.  
- Managers approve or decline claims.  
- HR can step in to log claims in emergencies and manage all users.  

