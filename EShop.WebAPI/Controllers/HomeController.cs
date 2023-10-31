using Microsoft.AspNetCore.Mvc;

namespace EShop.WebAPI.Controllers;

public sealed class HomeController : Controller
{
	public IActionResult Index()
	{
		return View();
	}
}