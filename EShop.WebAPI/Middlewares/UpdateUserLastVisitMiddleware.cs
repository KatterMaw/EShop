using System.Security.Claims;
using EShop.Data;
using EShop.Domain.Model;
using FlakeId;
using Microsoft.EntityFrameworkCore;
using Serilog;
using ILogger = Serilog.ILogger;

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
	
	private readonly AppDbContext _dbContext;
	private readonly ILogger _logger = Log.ForContext<UpdateUserLastVisitMiddleware>();

	private static bool IsAuthenticated(HttpContext context)
	{
		return context.User.Identity?.IsAuthenticated ?? false;
	}

	private async Task<User?> GetUser(HttpContext context)
	{
		var idClaim = context.User.FindFirstValue(ClaimTypes.Anonymous);
		if (idClaim != null)
		{
			var id = new Id(long.Parse(idClaim));
			return await _dbContext.GuestUsers.FirstAsync(user => user.Id == id);
		}
		idClaim = context.User.FindFirstValue("Id");
		if (idClaim != null)
		{
			var id = new Id(long.Parse(idClaim));
			return await _dbContext.AuthenticatedUsers.FirstAsync(user => user.Id == id);
		}
		return null;
	}
}