using Hotebedscache.Domain.Models.Requests;
using Hotebedscache.Domain.Models.Responses;
namespace Hotebedscache.Service.Interfaces
{
    public interface IAccountService
    {
        APIResponse<UserResponse> Login(UserRequest request, string secret);
        APIResponse<UserResponse> RefreshToken(string token, string secret);
        Task<APIResponse<UserResponse>> Register(RegistrationRequest request, string secret);
        APIResponse<OtpResponse> SendOtp(OtpRequest request);
        APIResponse<OtpResponse> VerifyOtp(OtpVerifyRequest request); 
        APIResponse<BaseUser> SaveProfile(BaseUser request);
        bool UpdateEmailVerified(string mobile); 
    }
}