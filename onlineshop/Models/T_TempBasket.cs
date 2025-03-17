using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace onlineshop.Models
{
    public class T_TempBasket
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid ID_TempBasket { get; set; }

        public Guid T_Product_ID { get; set; }
        [ForeignKey("T_Product_ID")]
        public T_Product Product { get; set; }

        public int Quantity { get; set; }

        public long TotalPrice { get; set; }

        public Guid T_tempOrder_ID { get; set; }
        [ForeignKey("T_tempOrder_ID")]

        public T_TempOrder TempOrder { get; set; }

    }
}
