using System.ComponentModel.DataAnnotations;

namespace WebApplication_ShopItemGame.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        public string UserId { get; set; }  // เชื่อมกับผู้ใช้ (Identity หรือระบบของคุณ)

        public DateTime OrderDate { get; set; }

        public decimal TotalAmount { get; set; }

        public string Status { get; set; } // เช่น "Pending", "Paid", "Shipped"

        public ICollection<OrderItem> OrderItems { get; set; }
    }
}
