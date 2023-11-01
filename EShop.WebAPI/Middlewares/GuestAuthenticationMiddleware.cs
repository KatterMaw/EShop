using System.Security.Claims;
using EShop.Data;
using EShop.Domain.Model;
using FlakeId;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Serilog;

namespace EShop.WebAPI.Middlewares;

public sealed class GuestAuthenticationMiddleware : IMiddleware
{
	public GuestAuthenticationMiddleware(AppDbContext dbContext)
	{
		_dbContext = dbContext;
	}
	
	public async Task InvokeAsync(HttpContext context, RequestDelegate next)
	{
		if (!IsAuthenticated(context))
		{
			GuestUser user = new();
			await CreateIdentity(context, user.Id);
			await _dbContext.GuestUsers.AddAsync(user);
			await _dbContext.SaveChangesAsync();
			_logger.Information("Guest authenticated with id {Id}", user.Id);
		}
		await next(context);
	}
	
	private readonly Serilog.ILogger _logger = Log.ForContext<GuestAuthenticationMiddleware>();
	private readonly AppDbContext _dbContext;
	
	private static bool IsAuthenticated(HttpContext context)
	{
		return context.User.Identity?.IsAuthenticated ?? false;
	}

	private static async Task CreateIdentity(HttpContext context, Id id)
	{
		ClaimsIdentity claimsIdentity = new(new Claim[]
		{
			new("Id", id.ToString())
		}, CookieAuthenticationDefaults.AuthenticationScheme);
		context.User.AddIdentity(claimsIdentity);
		await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
	}
}