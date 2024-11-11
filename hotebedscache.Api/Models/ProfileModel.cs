namespace Hotebedscache.Api.Models
{ 
    public class ProfileModel
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
}
