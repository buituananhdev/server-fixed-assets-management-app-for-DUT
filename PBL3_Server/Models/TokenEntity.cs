using System.ComponentModel.DataAnnotations;

namespace PBL3_Server.Models
{
    public class TokenEntity
    {
        [Key]
        public int Id { get; set; }
        public string UserID { get; set; }
        public string Username { get; set; }
        public string JwtToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpirationTime { get; set; }
        public DateTime AccessTime { get; set; }
    }
}
