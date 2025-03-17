using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace onlineshop.Models
{
    public class T_TempOrder
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid ID_TempOrder { get; set; }
        public string T_User_ID { get; set; }
        public DateTime CreatedAt { get; set; }

        public ICollection<T_TempBasket> Items { get; set; } = new List<T_TempBasket>();



    }
}
