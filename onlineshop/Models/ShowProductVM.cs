using System.ComponentModel.DataAnnotations;

namespace onlineshop.Models
{
    public class ShowProductVM
    {

        public string Name { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public long Price { get; set; }
        public bool IsAvailable { get; set; }

        public int Discount { get; set; }
        public bool IsDiscountActive { get; set; }

        public ICollection<T_L_ProductPhoto> Photos { get; set; } = new List<T_L_ProductPhoto>();


    }
}
