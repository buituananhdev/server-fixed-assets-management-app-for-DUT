using System.ComponentModel.DataAnnotations;

namespace PBL3_Server.Models
{
    public class User
    {
        [Key]
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string UserRole { get; set; } = string.Empty;
    }
}
