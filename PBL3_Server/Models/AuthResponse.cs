namespace PBL3_Server.Models
{
    public class AuthResponse
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public string Username { get; set; }
        public string Role { get; set; }
        public DateTime ExpirationTime { get; set; }
        public DateTime AccessTime { get; set; }
    }
}
