using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication_ShopItemGame.Data;

namespace WebApplication_ShopItemGame.Controllers
{
    public class ShopController : BaseController
    {
        private readonly ApplicationDbContext _db;

        public ShopController(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _db.Products.ToListAsync();
            return View(products);
        }

        public async Task<IActionResult> Details(int id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Auth");
            }

            var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return NotFound();
            return View(product);
        }

    }
}

