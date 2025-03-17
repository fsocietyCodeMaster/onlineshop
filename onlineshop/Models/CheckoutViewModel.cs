using System.ComponentModel.DataAnnotations;

namespace onlineshop.Models
{
    public class CheckoutViewModel
    {
        [Required]
        public required string Name { get; set; }

        [Required]
        public required string City { get; set; }

        [Required]
        public required string Address { get; set; }

    }
}