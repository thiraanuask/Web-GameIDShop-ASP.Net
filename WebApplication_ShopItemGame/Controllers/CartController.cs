using Microsoft.AspNetCore.Mvc;
using WebApplication_ShopItemGame.Data;
using WebApplication_ShopItemGame.Helpers;
using WebApplication_ShopItemGame.Models;

namespace WebApplication_ShopItemGame.Controllers
{
    public class CartController : BaseController
    {
        private readonly ApplicationDbContext _db;
        private const string SessionCartKey = "SessionCart";

        public CartController(ApplicationDbContext db) : base(db) 
        {
            _db = db;
        }

        // แสดงตะกร้าสินค้า
        public IActionResult Index()
        {
            var cart = HttpContext.Session.GetObject<List<CartItem>>(SessionCartKey) ?? new List<CartItem>();
            return View(cart);
        }

        // เพิ่มสินค้าใส่ตะกร้า
        [HttpPost]
        public IActionResult AddToCart(int productId, int quantity = 1)
        {
            var product = _db.Products.Find(productId);
            if (product == null)
                return NotFound();

            var cart = HttpContext.Session.GetObject<List<CartItem>>(SessionCartKey) ?? new List<CartItem>();

            var existingItem = cart.FirstOrDefault(c => c.ProductId == productId);
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                cart.Add(new CartItem { ProductId = productId, Product = product, Quantity = quantity });
            }

            HttpContext.Session.SetObject(SessionCartKey, cart);

            return RedirectToAction("Index", "Cart");
        }

        // ลบสินค้าจากตะกร้า
        public IActionResult Remove(int productId)
        {
            var cart = HttpContext.Session.GetObject<List<CartItem>>(SessionCartKey);
            if (cart == null) return RedirectToAction("Index");

            var item = cart.FirstOrDefault(c => c.ProductId == productId);
            if (item != null)
            {
                cart.Remove(item);
                HttpContext.Session.SetObject(SessionCartKey, cart);
            }
            return RedirectToAction("Index");
        }

        // เคลียร์ตะกร้า
        public IActionResult Clear()
        {
            HttpContext.Session.Remove(SessionCartKey);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult AddToCartAjax(int productId, int quantity = 1)
        {

            var product = _db.Products.FirstOrDefault(p => p.Id == productId);
            if (product == null)
                return Json(new { success = false, message = "สินค้าไม่พบ" });

            var cart = HttpContext.Session.GetObject<List<CartItem>>("SessionCart") ?? new List<CartItem>();

            var existingItem = cart.FirstOrDefault(c => c.ProductId == productId);
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                cart.Add(new CartItem { ProductId = productId, Product = product, Quantity = quantity });
            }

            HttpContext.Session.SetObject("SessionCart", cart);

            return Json(new { success = true, message = "เพิ่มสินค้าในตะกร้าแล้ว", cartCount = cart.Sum(c => c.Quantity) });
        }

    }
}
