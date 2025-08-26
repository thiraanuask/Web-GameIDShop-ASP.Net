using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication_ShopItemGame.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "กรุณากรอกชื่อสินค้า")]
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }

        [Required(ErrorMessage = "กรุณากรอกราคา")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
        public int Stock { get; set; }
    }
}
