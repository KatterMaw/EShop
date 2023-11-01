using System.Security.Claims;
using FlakeId;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace EShop.WebAPI.Extensions;

public static class HttpContextExtensions
{
	public static Task CreateIdentity(this HttpContext context, Id id)
	{
		ClaimsIdentity claimsIdentity = new(new Claim[]
		{
			new("Id", id.ToString())
		}, CookieAuthenticationDefaults.AuthenticationScheme);
		context.User.AddIdentity(claimsIdentity);
		return context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
	}
}