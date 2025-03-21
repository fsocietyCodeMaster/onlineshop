using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace onlineshop.Models
{
    public class T_L_ProductPhoto
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid ID_ProductPhoto { get; set; }
        public string ImageUrl { get; set; }
        public Guid T_Product_ID { get; set; }
        [ForeignKey("T_Product_ID")]
        [JsonIgnore]
        public T_Product T_Product { get; set; }

    }
}
