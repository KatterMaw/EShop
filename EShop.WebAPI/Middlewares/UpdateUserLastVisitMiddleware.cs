using EShop.Application;
using EShop.WebAPI.Extensions;

namespace EShop.WebAPI.Middlewares;

public sealed class UpdateUserLastVisitMiddleware : IMiddleware
{
	public UpdateUserLastVisitMiddleware(UsersProvider usersProvider, UserLastVisitUpdater userLastVisitUpdater)
	{
		_usersProvider = usersProvider;
		_userLastVisitUpdater = userLastVisitUpdater;
	}
	
	public async Task InvokeAsync(HttpContext context, RequestDelegate next)
	{
		if (IsAuthenticated(context))
		{
			var user = await _usersProvider.GetHttpContextUser(context);
			await _userLastVisitUpdater.UpdateLastVisit(user);
		}
		await next(context);
	}

	private readonly UsersProvider _usersProvider;
	private readonly UserLastVisitUpdater _userLastVisitUpdater;

	private static bool IsAuthenticated(HttpContext context)
	{
		return context.User.Identity?.IsAuthenticated ?? false;
	}
}