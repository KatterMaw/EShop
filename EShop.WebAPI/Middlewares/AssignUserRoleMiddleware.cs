using System.Security.Claims;
using CommunityToolkit.Diagnostics;
using EShop.Application;
using EShop.Domain.Model;
using EShop.WebAPI.Extensions;

namespace EShop.WebAPI.Middlewares;

public sealed class AssignUserRoleMiddleware : IMiddleware
{
	public AssignUserRoleMiddleware(UsersProvider usersProvider)
	{
		_usersProvider = usersProvider;
	}
	
	public async Task InvokeAsync(HttpContext context, RequestDelegate next)
	{
		var user = await _usersProvider.GetHttpContextUser(context);
		var role = user switch
		{
			GuestUser => Roles.Guest,
			AuthenticatedUser => Roles.User,
			_ => ThrowHelper.ThrowArgumentOutOfRangeException<string>(nameof(user), user.GetType(), "Unknown user type")
		};
		context.User.AddIdentity(new ClaimsIdentity(new[] {new Claim(ClaimTypes.Role, role)}));
		await next(context);
	}
	
	private readonly UsersProvider _usersProvider;
}