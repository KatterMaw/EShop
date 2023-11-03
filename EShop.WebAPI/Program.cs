using EShop.Application;
using EShop.Data;
using EShop.Services;
using EShop.Services.UsersProviders;
using EShop.WebAPI.Middlewares;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
	.CreateBootstrapLogger();

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
builder.Services.AddTransient<UpdateUserLastVisitMiddleware>();
builder.Services.AddTransient<AssignUserRoleMiddleware>();
builder.Services.AddScoped<UsersProvider, IdCachedDbUsersProvider>();
builder.Services.AddScoped<UserLastVisitUpdater, DbUserLastVisitUpdater>();
builder.Services.AddAuthorization();
builder.Services.AddControllersWithViews();
builder.Host.UseSerilog((context, services, configuration) =>
{
	configuration.ReadFrom.Configuration(context.Configuration);
	configuration.ReadFrom.Services(services);
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
	var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
	await dbContext.Database.EnsureCreatedAsync();
	await dbContext.SeedData();
}

app.UseSerilogRequestLogging();
app.UseAuthentication();
app.UseMiddleware<UpdateUserLastVisitMiddleware>();
app.UseMiddleware<GuestAuthenticationMiddleware>();
app.UseMiddleware<AssignUserRoleMiddleware>();
app.UseAuthorization();
app.MapControllerRoute("default", "{controller=Home}/{action=Index}");

app.MapGet("/AccessDenied", context =>
{
	context.Response.StatusCode = StatusCodes.Status403Forbidden;
	return context.Response.WriteAsync("Access Denied");
});

app.Run();