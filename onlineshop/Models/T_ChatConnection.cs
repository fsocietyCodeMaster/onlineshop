using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace onlineshop.Models
{
    public class T_ChatConnection
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }
        public string ConnectionId { get; set; }
        public string UserId { get; set; }
        public string Role { get; set; }
        public string? AssignedAdminId { get; set; } // just for users  
        public string? LastAssignedAdminId { get; set; } // just for users
    }
}
