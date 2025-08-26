using Microsoft.AspNetCore.Mvc;
using WebApplication_ShopItemGame.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace WebApplication_ShopItemGame.Controllers
{
    [Authorize]
    public class DepositController : BaseController
    {
        private readonly ApplicationDbContext _db;

        public DepositController(ApplicationDbContext db) : base(db) 
        {
            _db = db;
        }

        // GET: Deposit
        public async Task<IActionResult> Index()
        {
            // ดึงข้อมูลผู้ใช้ปัจจุบัน
            var username = User.Identity.Name;
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null)
                return NotFound();

            return View(user);
        }

        // POST: Deposit/Add
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(decimal amount)
        {
            if (amount <= 0)
            {
                ModelState.AddModelError("", "จำนวนเงินต้องมากกว่า 0");
                return RedirectToAction("Index");
            }

            var username = User.Identity.Name;
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null)
                return NotFound();

            user.Balance += amount;
            _db.Update(user);
            await _db.SaveChangesAsync();

            TempData["Success"] = $"เติมเงิน {amount:C} สำเร็จ ยอดเงินคงเหลือ {user.Balance:C}";

            return RedirectToAction("Index");
        }

    }
}
