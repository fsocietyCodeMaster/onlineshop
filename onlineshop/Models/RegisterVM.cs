using System.ComponentModel.DataAnnotations;

namespace onlineshop.Models
{
    public class RegisterVM
    {
        [Required(ErrorMessage ="Enter your Fullname")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Enter your Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Enter your Password")]
        public string Password { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }

        public int Age { get; set; }
    }
}
