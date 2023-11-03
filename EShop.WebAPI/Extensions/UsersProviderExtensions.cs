using EShop.Application;
using EShop.Domain.Model;
using FlakeId;

namespace EShop.WebAPI.Extensions;

public static class UsersProviderExtensions
{
	public static Task<User> GetHttpContextUser(this UsersProvider usersProvider, HttpContext context)
	{
		var idClaim = context.User.Claims.Single(claim => claim.Type == "Id");
		Id id = new(long.Parse(idClaim.Value));
		return usersProvider.GetUser(id);
	}
}