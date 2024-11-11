using Hotebedscache.Domain.Entities;

namespace Hotebedscache.Domain.Models.Responses
{

    public class BaseUser
    {
        public int UserId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Mobile { get; set; }
        public string? Role { get; set; }
        public string? Logo { get; set; }
        public string? Address { get; set; }
        public int CityId { get; set; }
        public bool IsMobileVerified { get; set; }
        public bool IsEmailVerified { get; set; }
    }
    public class UserResponse : BaseUser
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; } 
        
        public UserResponse(User user, string token, string refreshToken)
        {
            UserId = user.Id;
            IsMobileVerified = user.MobileVerified;
            IsEmailVerified = user.EmailVerified;
            Email = user.Email;
            Token = token;
            Mobile = user.Mobile;
            RefreshToken = refreshToken;
            Role = user.Role;
        }
    }

    public class AccountResponse
    { 
        public string? Address { get; set; } 
        public string? City { get; set; } 
        public string? Mobile { get; set; }  
    }
}
