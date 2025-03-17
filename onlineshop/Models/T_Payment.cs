using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace onlineshop.Models
{
    public class T_Payment
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid PaymentId { get; set; }
        public string? Message { get; set; }

        public long? TrackId { get; set; }

        public string? Description { get; set; }

        public DateTime PaidAt { get; set; }

        public Guid T_Order_ID { get; set; }

        [ForeignKey("T_Order_ID")]
        public T_Order? Order { get; set; }

    }
}
