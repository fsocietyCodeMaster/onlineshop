using System.ComponentModel.DataAnnotations;

namespace onlineshop.Models
{
    public class LoginVM
    {
        [StringLength(30)]
        [Required(ErrorMessage ="Username must be your email")]
        public string Username { get; set; }

        [StringLength(50)]
        [DataType(DataType.Password)]
        [Required]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
        public string? ReturnUrl { get; set; }
    }
}
