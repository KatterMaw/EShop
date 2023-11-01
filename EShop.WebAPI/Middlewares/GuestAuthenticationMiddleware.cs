using EShop.Data;
using EShop.Domain.Model;
using EShop.WebAPI.Extensions;
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
			await context.CreateIdentity(user.Id);
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
}