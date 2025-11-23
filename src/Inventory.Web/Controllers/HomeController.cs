using Microsoft.AspNetCore.Mvc;

namespace Inventory.Web.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Dashboard()
    {
        return View();
    }

    public IActionResult Error()
    {
        return View();
    }
}
