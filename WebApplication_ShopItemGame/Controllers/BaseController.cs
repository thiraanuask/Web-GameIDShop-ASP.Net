using Microsoft.AspNetCore.Mvc;
using WebApplication_ShopItemGame.Data;
using WebApplication_ShopItemGame.Helpers;
using WebApplication_ShopItemGame.Models;

namespace WebApplication_ShopItemGame.Controllers
{
    public class BaseController : Controller
    {
        protected readonly ApplicationDbContext _db;

        public BaseController(ApplicationDbContext db)
        {
            _db = db;
        }

        public override void OnActionExecuting(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            if (User.Identity.IsAuthenticated)
            {
                var username = User.Identity.Name;
                var user = _db.Users.FirstOrDefault(u => u.Username == username);
                if (user != null)
                {
                    ViewData["UserBalance"] = user.Balance;
                }
            }

            var cart = HttpContext.Session.GetObject<List<CartItem>>("SessionCart") ?? new List<CartItem>();
            ViewData["CartCount"] = cart.Sum(c => c.Quantity);

        }
    }
}
