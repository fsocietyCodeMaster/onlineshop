

namespace onlineshop.Models
{
    public class BasketViewModel
    {
        public T_Product Product { get; set; }

        public int Quantity { get; set; }

        public long TotalPrice { get; set; }
    }
}