using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace onlineshop.Models
{
    public class T_TempBasket
    {

        public Guid ID_TempBasket { get; set; } = Guid.NewGuid();

        public Guid T_Product_ID { get; set; }
        public T_Product Product { get; set; }

        public int Quantity { get; set; }

        public long TotalPrice { get; set; }

        public Guid T_tempOrder_ID { get; set; }
        public T_TempOrder TempOrder { get; set; }

    }
}
