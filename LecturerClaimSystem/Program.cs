using LecturerClaimSystem.Data;
using LecturerClaimSystem.Models;
using LecturerClaimSystem.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

var dbPath = Path.Combine(builder.Environment.ContentRootPath, "cmcs.db");
builder.Services.AddDbContext<AppDbContext>(options =>
	options.UseSqlite($"Data Source={dbPath}"));

builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
	options.Password.RequireDigit = false;
	options.Password.RequireUppercase = false;
	options.Password.RequireLowercase = false;
	options.Password.RequireNonAlphanumeric = false;
	options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
	options.AccessDeniedPath = "/Home/AccessDenied";
});

builder.Services.AddSingleton<FileStorageService>();
builder.Services.AddSingleton<ReportService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
	var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
	db.Database.Migrate();

	var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
	var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

	string[] roles = { "HR", "Lecturer", "Coordinator", "Manager" };
	foreach (var r in roles)
	{
		if (!await roleMgr.RoleExistsAsync(r))
			await roleMgr.CreateAsync(new IdentityRole(r));
	}

	var hrEmail = "hr@cmcs.local";
	if (await userMgr.FindByEmailAsync(hrEmail) is null)
	{
		var hr = new AppUser
		{
			UserName = hrEmail,
			Email = hrEmail,
			FirstName = "HR",
			LastName = "Admin",
			HourlyRate = 0,
			EmailConfirmed = true
		};
		await userMgr.CreateAsync(hr, "Pass@123");
		await userMgr.AddToRoleAsync(hr, "HR");
	}
}

if (app.Environment.IsDevelopment())
{
	app.UseDeveloperExceptionPage();
}
else
{
	app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();