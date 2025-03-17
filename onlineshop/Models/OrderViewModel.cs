
namespace onlineshop.Models
{
    public class OrderViewModel
    {
        public required string Name { get; set; }

        public required string City { get; set; }

        public required string Address { get; set; }

        public OrderStatus OrderStatus { get; set; }
        public required List<BasketViewModel> BasketDetails { get; set; }
    }
}