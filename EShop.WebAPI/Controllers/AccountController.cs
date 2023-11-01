using CommunityToolkit.Diagnostics;
using EShop.Data;
using EShop.Domain.Model;
using EShop.WebAPI.Extensions;
using FlakeId;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EShop.WebAPI.Controllers;

public sealed class AccountController : Controller
{
	public AccountController(AppDbContext dbContext)
	{
		_dbContext = dbContext;
	}
	
	[Authorize(Roles = Roles.User)]
	public IActionResult Index()
	{
		var user = _dbContext.AuthenticatedUsers.Find(
			new Id(long.Parse(HttpContext.User.Claims.Single(claim => claim.Type == "Id").Value)));
		Guard.IsNotNull(user);
		return Content(user.ToString());
	}
	
	[HttpGet]
	[Authorize(Roles = Roles.Guest)]
	public IActionResult SignIn()
	{
		return View();
	}

	[HttpPost]
	[Authorize(Roles = Roles.Guest)]
	public async Task<IActionResult> SignIn(string name, string password, string returnUrl = "")
	{
		var user = await _dbContext.AuthenticatedUsers.SingleOrDefaultAsync(user => user.Name == name);
		if (user == null)
			return View(model: $"User with name \"{name}\" not found");
		if (!user.Password.Verify(password))
			return View(model: $"Wrong password for user with name \"{name}\"");

		await HttpContext.CreateIdentity(user.Id);
		
		if (!string.IsNullOrEmpty(returnUrl))
			return Redirect(returnUrl);
		return RedirectToAction("Index", "Home");
	}
	
	[HttpGet]
	[Authorize(Roles = Roles.Guest)]
	public IActionResult SignUp()
	{
		return View();
	}

	[HttpPost]
	[Authorize(Roles = Roles.Guest)]
	public async Task<IActionResult> SignUp(string name, string password, string returnUrl = "")
	{
		var existingUser = await _dbContext.AuthenticatedUsers.SingleOrDefaultAsync(user => user.Name == name);
		if (existingUser != null)
			return View(model: $"User with name \"{name}\" already exists");
		
		AuthenticatedUser user = new(name, password);
		await _dbContext.AuthenticatedUsers.AddAsync(user);
		await _dbContext.SaveChangesAsync();

		await HttpContext.CreateIdentity(user.Id);
		
		if (!string.IsNullOrEmpty(returnUrl))
			return Redirect(returnUrl);
		return RedirectToAction("Index", "Home");
	}

	[Authorize(Roles = Roles.User)]
	public async Task<IActionResult> SignOut(string returnUrl = "")
	{
		await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
		if (!string.IsNullOrEmpty(returnUrl))
			return Redirect(returnUrl);
		return RedirectToAction("Index", "Home");
	}

	private readonly AppDbContext _dbContext;
}