using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace onlineshop.Models
{
    public class T_L_Category
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid ID_Category { get; set; }

        [Required(ErrorMessage = "Category's name is necessary.")]
        [MaxLength(50)]
        public string Name { get; set; }
        public bool IsActive { get; set; }

        public ICollection<T_Product> Products { get; set; }    

    }
}
