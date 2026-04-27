using Microsoft.AspNetCore.Mvc;
using SubastaAutos.Web.Filters;
using SubastaAutos.Web.Models;
using System.Diagnostics;

namespace SubastaAutos.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [RolAutorizado]

        public IActionResult Denegado()
        {
            return View();
        }
        [RolAutorizado]
        public IActionResult Index()
        {
            return View();
        }

        [RolAutorizado]
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
}
