using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace onlineshop.Models
{
    public class T_User : IdentityUser
    {
        [StringLength(25)]
        public string FullName { get; set; }
        [StringLength(100)]
        public string? Address { get; set; }

        public int Age { get; set; }
    }
}
