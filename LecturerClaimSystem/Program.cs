using LecturerClaimSystem.Data;
using LecturerClaimSystem.Models;
using LecturerClaimSystem.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();


builder.Services.AddDbContext<AppDbContext>(options =>
	options.UseSqlite($"Data Source={Path.Combine(builder.Environment.ContentRootPath, "cmcs.db")}"));

builder.Services.AddSingleton<FileStorageService>();
builder.Services.AddSingleton<ReportService>();


builder.Services.AddSession(options =>
{
	options.Cookie.Name = "CMCS.Session";
	options.IdleTimeout = TimeSpan.FromMinutes(30);
	options.Cookie.HttpOnly = true;
	options.Cookie.IsEssential = true;
});

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
	var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
	db.Database.EnsureCreated();
}

if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.UseSession(); // enable sessions
app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Lecturer}/{action=Dashboard}/{id?}");

app.Run();