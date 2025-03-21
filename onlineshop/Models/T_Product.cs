using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace onlineshop.Models
{
    public class T_Product
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
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
        [ForeignKey("T_Category_ID")]
        [JsonIgnore]
        public T_L_Category Category { get; set; }

        public ICollection<T_L_ProductPhoto> Photos { get; set; } = new List<T_L_ProductPhoto>();

    }
}
