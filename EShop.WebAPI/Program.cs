using EShop.Data;
using EShop.WebAPI.Middlewares;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<AppDbContext>(optionsBuilder =>
{
	var connectionStringBuilder = new SqliteConnectionStringBuilder
	{
		DataSource = "EShop.db"
	};
	optionsBuilder.UseSqlite(connectionStringBuilder.ConnectionString);
}, optionsLifetime: ServiceLifetime.Singleton);

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options => options.AccessDeniedPath = "/AccessDenied");
builder.Services.AddTransient<GuestAuthenticationMiddleware>();
builder.Services.AddAuthorization();
builder.Services.AddControllersWithViews();
builder.Services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog());

var app = builder.Build();

Log.Logger = new LoggerConfiguration()
	.ReadFrom.Configuration(app.Configuration)
	.CreateLogger();

using (var scope = app.Services.CreateScope())
{
	await scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.EnsureCreatedAsync();
}

app.UseAuthentication();
app.UseMiddleware<GuestAuthenticationMiddleware>();
app.UseAuthorization();
app.MapControllerRoute("default", "{controller=Home}/{action=Index}");

app.MapGet("/AccessDenied", context =>
{
	context.Response.StatusCode = StatusCodes.Status403Forbidden;
	return context.Response.WriteAsync("Access Denied");
});

app.Run();