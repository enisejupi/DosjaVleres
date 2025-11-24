using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using KosovaDoganaModerne.Models;
using KosovaDoganaModerne.Depo;

namespace KosovaDoganaModerne.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IDepoja_VleraProduktit _depoja;

    public HomeController(ILogger<HomeController> logger, IDepoja_VleraProduktit depoja)
    {
        _logger = logger;
        _depoja = depoja;
    }

    public async Task<IActionResult> Index()
    {
        var produkte = await _depoja.MerrTeGjitha();
        ViewBag.TotalProducts = produkte.Count();
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
