using System.ComponentModel.DataAnnotations;

namespace onlineshop.Models
{
    public class Update_ProductVM
    {
        public Guid ID_Product { get; set; }

        [Required(ErrorMessage = "Product's name is necessary.")]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Description is necessary.")]
        [MaxLength(200)]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [Required(ErrorMessage = "Quantity is necessary.")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Price is necessary.")]
        [Range(1000, long.MaxValue, ErrorMessage = "Price must be a positive value.")]
        public long Price { get; set; }
        public bool IsAvailable { get; set; }

        public int? Discount { get; set; }
        public bool IsDiscountActive { get; set; }

        public Guid T_Category_ID { get; set; }
        public ICollection<IFormFile>? ImageUrl { get; set; }
    }
}
