using onlineshop.Repositroy;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace onlineshop.Models
{
    public class T_Basket
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid ID_Basket { get; set; }

        public Guid ProductId { get; set; }
        [ForeignKey("ProductId")]
        public T_Product Product { get; set; }
        public int Quantity { get; set; }

        public long TotalPrice { get; set; }

        public Guid T_Order_ID { get; set; }
        [ForeignKey("T_Order_ID")]
        public T_Order Order { get; set; }


    }
}
