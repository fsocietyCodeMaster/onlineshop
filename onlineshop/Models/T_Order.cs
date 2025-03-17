using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace onlineshop.Models
{
    public class T_Order
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid ID_Order { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public bool IsFinal { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public DateTime OrderDate { get; set; }

        public string T_User_ID { get; set; }

        [ForeignKey("T_User_ID")]
        public T_User User { get; set; }
        public ICollection<T_Basket> Baskets { get; set; } = new List<T_Basket>();

    }
    public enum OrderStatus
    {

        [Display(Name = "Order Pending")]
        PENDING,

        [Display(Name = "Order Processing")]
        PROCESSING,

        [Display(Name = "Order Sending")]
        SENDING,

        [Display(Name = "Order Delivered")]
        DELIVERED

    }
}
