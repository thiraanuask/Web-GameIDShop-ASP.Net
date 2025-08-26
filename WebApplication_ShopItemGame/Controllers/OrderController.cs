using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication_ShopItemGame.Data;
using WebApplication_ShopItemGame.Models;
using Microsoft.EntityFrameworkCore;
using WebApplication_ShopItemGame.Helpers;

namespace WebApplication_ShopItemGame.Controllers
{
    [Authorize]
    public class OrderController : BaseController 
    {

        private readonly ApplicationDbContext _db;
        private const string SessionCartKey = "SessionCart";

        public OrderController(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        
        public IActionResult Checkout()
        {
            var cart = HttpContext.Session.GetObject<List<CartItem>>(SessionCartKey);
            if (cart == null || !cart.Any())
                return RedirectToAction("Index", "Cart");

            return View(cart);
        }

        
        [HttpPost]
        public async Task<IActionResult> PlaceOrder()
        {
            var cart = HttpContext.Session.GetObject<List<CartItem>>(SessionCartKey);
            if (cart == null || !cart.Any())
                return RedirectToAction("Index", "Cart");

          
            var order = new Order
            {
                UserId = User.Identity.Name, 
                OrderDate = DateTime.Now,
                Status = "Pending",
                TotalAmount = cart.Sum(i => i.Product.Price * i.Quantity),
                OrderItems = new List<OrderItem>()
            };

            foreach (var item in cart)
            {
                order.OrderItems.Add(new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.Product.Price
                });

                
                var product = await _db.Products.FindAsync(item.ProductId);
                if (product != null)
                {
                    product.Stock -= item.Quantity;
                    _db.Products.Update(product);
                }
            }
           
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == User.Identity.Name);
            if (user == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            if (user.Balance < order.TotalAmount)
            {
                ModelState.AddModelError("", "ยอดเงินไม่เพียงพอ กรุณาเติมเงินก่อนสั่งซื้อ");
                return View("Checkout", cart);
            }

            user.Balance -= order.TotalAmount;
            _db.Users.Update(user);

            _db.Orders.Add(order);
            await _db.SaveChangesAsync();

            
            HttpContext.Session.Remove(SessionCartKey);

            
            return RedirectToAction("OrderSuccess", new { id = order.Id });
        }

        
        public async Task<IActionResult> OrderSuccess(int id)
        {
            var order = await _db.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return NotFound();

            order.Status = "Success";
            await _db.SaveChangesAsync();

            return View(order);
        }

        public IActionResult CartCountPartial()
        {
            var cart = HttpContext.Session.GetObject<List<CartItem>>("SessionCart") ?? new List<CartItem>();
            ViewData["CartCount"] = cart.Sum(i => i.Quantity);
            return PartialView("_CartCountPartial");
        }
    }
}
