using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebApplication_ShopItemGame.Data;
using WebApplication_ShopItemGame.Models;

namespace WebApplication_ShopItemGame.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ApplicationDbContext db, ILogger<HomeController> logger)
            : base(db)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var products = _db.Products.ToList();
            if (products == null) products = new List<Product>();
            return View(products);

      
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
