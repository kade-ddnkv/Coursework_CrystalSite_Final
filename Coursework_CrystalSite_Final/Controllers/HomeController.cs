using Coursework_CrystalSite_Final.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Coursework_CrystalSite_Final.Controllers
{
    /// <summary>
    /// Контроллер, отвечающий за главную (вводную) страницу.
    /// </summary>
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Возвращает представление главной (вводной) страницы сайта.
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
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
}