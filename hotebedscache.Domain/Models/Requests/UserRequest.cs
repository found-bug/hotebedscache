namespace Hotebedscache.Domain.Models.Requests
{
    public class UserRequest
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
    }
    public class RefreshTokenRequest
    {
        public string? Token { get; set; }
    }
}
