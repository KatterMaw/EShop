using EShop.Data;
using FlakeId;
using Microsoft.AspNetCore.Mvc;

namespace EShop.WebAPI.Controllers;

public sealed class HomeController : Controller
{
	public HomeController(AppDbContext dbContext)
	{
		_dbContext = dbContext;
	}

	public async Task<IActionResult> Index()
	{
		var user = await _dbContext.Users.FindAsync(new Id(long.Parse(HttpContext.User.Claims.Single(claim => claim.Type == "Id").Value)));
		return View(user);
	}

	private readonly AppDbContext _dbContext;
}