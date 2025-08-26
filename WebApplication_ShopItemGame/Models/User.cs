using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication_ShopItemGame.Models
{
    public class User
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string PasswordHash { get; set; }
        [NotMapped] // ไม่เก็บในฐานข้อมูล 
        [Required(ErrorMessage = "Please confirm password")]
        [DataType(DataType.Password)]
        [Compare("PasswordHash", ErrorMessage = "Password and confirmation do not match.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email address")]
        public string Email { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Balance { get; set; } = 0;
    }
}

