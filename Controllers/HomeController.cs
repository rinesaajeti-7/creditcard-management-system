using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CreditCard.Models;

namespace CreditCard.Controllers;

public class HomeController : Controller
{
    // Deklarimi i logger-it për regjistrimin e ngjarjeve
    private readonly ILogger<HomeController> _logger;

    // Konstruktori që merr logger si dependency injection
    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    // Metoda GET për faqen kryesore të aplikacionit
    public IActionResult Index()
    {
        return View();
    }

    // Metoda GET për faqen e politikës së privatësisë
    public IActionResult Privacy()
    {
        return View();
    }

    // Metoda GET për faqen e gabimeve
    // Atributi ResponseCache parandalon caching e kësaj faqeje
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        // Krijon ErrorViewModel me RequestId nga aktiviteti aktual ose trace identifier
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}