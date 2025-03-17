namespace onlineshop.Models
{
    public class PaymentViewModel
    {
        public string? message { get; set; }

        public string? description { get; set; }

        public DateTime PaidAt { get; set; }

        public Guid orderId { get; set; }
    }
}
