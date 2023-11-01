using System.Security.Claims;
using EShop.Data;
using EShop.Domain.Model;
using FlakeId;
using Serilog;

namespace EShop.WebAPI.Middlewares;

public sealed class UpdateUserLastVisitMiddleware : IMiddleware
{
	public UpdateUserLastVisitMiddleware(AppDbContext dbContext)
	{
		_dbContext = dbContext;
	}
	
	public async Task InvokeAsync(HttpContext context, RequestDelegate next)
	{
		if (IsAuthenticated(context))
		{
			var user = await GetUser(context);
			if (user != null)
			{
				var oldLastVisit = user.LastVisit;
				user.UpdateLastVisit();
				await _dbContext.SaveChangesAsync();
				_logger.Information("User {User} last visit updated from {OldLastVisit} to {NewLastVisit}", user, oldLastVisit, user.LastVisit);
			}
		}
		await next(context);
	}
	
	private readonly Serilog.ILogger _logger = Log.ForContext<UpdateUserLastVisitMiddleware>();
	private readonly AppDbContext _dbContext;

	private static bool IsAuthenticated(HttpContext context)
	{
		return context.User.Identity?.IsAuthenticated ?? false;
	}

	private async Task<User?> GetUser(HttpContext context)
	{
		var idClaim = context.User.FindFirstValue("Id");
		if (idClaim != null)
		{
			var id = new Id(long.Parse(idClaim));
			var user = await _dbContext.Users.FindAsync(id);
			if (user == null)
				_logger.Warning("User with id {Id} not found", id);
			return user;
		}
		_logger.Warning("User id (claim) not found");
		return null;
	}
}