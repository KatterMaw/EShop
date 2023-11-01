using System.Security.Claims;
using CommunityToolkit.Diagnostics;
using EShop.Data;
using EShop.Domain.Model;
using FlakeId;
using Serilog;

namespace EShop.WebAPI.Middlewares;

public sealed class AssignUserRoleMiddleware : IMiddleware
{
	public AssignUserRoleMiddleware(AppDbContext dbContext)
	{
		_dbContext = dbContext;
	}
	
	public async Task InvokeAsync(HttpContext context, RequestDelegate next)
	{
		var user = await GetUser(context);
		if (user == null)
			return;
		var role = user switch
		{
			GuestUser => Roles.Guest,
			AuthenticatedUser => Roles.User,
			_ => ThrowHelper.ThrowArgumentOutOfRangeException<string>(nameof(user), user.GetType(), "Unknown user type")
		};
		context.User.AddIdentity(new ClaimsIdentity(new[] {new Claim(ClaimTypes.Role, role)}));
		await next(context);
	}
	
	private readonly Serilog.ILogger _logger = Log.ForContext<AssignUserRoleMiddleware>();
	private readonly AppDbContext _dbContext;
	
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