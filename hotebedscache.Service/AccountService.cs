using Hotebedscache.Domain.Common;
using Hotebedscache.Service.Common;
using Hotebedscache.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Hotebedscache.Domain.Models.Responses;
using Hotebedscache.Domain.Models.Requests;
using Hotebedscache.Domain.Entities;
namespace Hotebedscache.Service
{
    public class AccountService : IAccountService
    {
        private readonly HotebedContext context;
        public AccountService(HotebedContext context)
        {
            this.context = context;
        }

        #region Account
        public APIResponse<UserResponse> Login(UserRequest request, string secret)
        {
            var response = new APIResponse<UserResponse>();
            try
            {
                UserResponse user;
                User userInfo = null;
                if (request.Email.Contains('@') || request.Email.Contains('.'))
                {
                    userInfo = context.Users.FirstOrDefault(a => a.Email == request.Email && a.Password == request.Password);
                }
                else
                {
                    var mobileno = request.Email.Substring(request.Email.Length - 10);
                    if (mobileno != null && mobileno != string.Empty) userInfo = context.Users.FirstOrDefault(a => a.Mobile.Contains(mobileno) && a.Password == request.Password);
                }
                if (userInfo != null)
                {
                    var Token = GenerateToken(secret, userInfo);
                    var refreshToken = GenerateRefreshToken();
                    user = new UserResponse(userInfo, Token, refreshToken.Token);
                    refreshToken.UserId = userInfo.Id;
                    context.RefreshTokens.Add(refreshToken);
                    context.Update(userInfo);
                    context.SaveChanges(); 
                    response.Result = user;
                }
                else
                {
                    response.FailResult("Invalid credentails");
                }
            }
            catch (Exception e)
            {
                response.FailResult(e);
            }
            return response;
        }

        public APIResponse<UserResponse> RefreshToken(string token, string secret)
        {
            var response = new APIResponse<UserResponse>();
            try
            {
                var refreshToken = context.RefreshTokens.FirstOrDefault(a => a.Token == token);
                if (refreshToken == null) { response.FailResult("invalid token"); return response; }
                var user = context.Users.SingleOrDefault(u => u.Id == refreshToken.UserId);

                // return null if no user found with token
                if (user == null) { response.FailResult("invalid token"); return response; }


                // replace old refresh token with a new one and save
                var newRefreshToken = GenerateRefreshToken();
                newRefreshToken.UserId = user.Id;
                refreshToken.Revoked = DateTime.UtcNow;
                refreshToken.ReplacedBy = newRefreshToken.Token;

                context.RefreshTokens.Add(newRefreshToken);
                context.Update(refreshToken);
                context.SaveChanges();

                // generate new jwt
                var jwtToken = GenerateToken(secret, user);
                var userIno = new UserResponse(user, jwtToken, newRefreshToken.Token);
                response.Result = userIno;
            }
            catch (Exception)
            {
                response.FailResult("error while refreshing token");
            }
            return response;

        }

        private static string GenerateToken(string secret, User userInfo)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                         new Claim("UserId",userInfo.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        private RefreshToken GenerateRefreshToken()
        {
            using (var rngCryptoServiceProvider = new RNGCryptoServiceProvider())
            {
                var randomBytes = new byte[64];
                rngCryptoServiceProvider.GetBytes(randomBytes);
                return new RefreshToken
                {
                    Token = Convert.ToBase64String(randomBytes),
                    Expires = DateTime.UtcNow.AddDays(7),
                    Created = DateTime.UtcNow
                };
            }
        }

        public async Task<APIResponse<UserResponse>> Register(RegistrationRequest request, string secret)
        {
            var response = new APIResponse<UserResponse>();
            try
            {
                UserResponse user;
                var userInfo = context.Users.FirstOrDefault(a => a.Email == request.Email);
                if (userInfo != null)
                {
                    response.FailResult("Email already registered");
                    return response;
                }

                User newUser = new User();
                newUser.Email = request.Email;
                newUser.EmailVerified = false;
                newUser.MobileVerified = false;
                newUser.Password = request.Password;
                newUser.Mobile = request.Mobile;
                context.Add<User>(newUser);
                context.SaveChanges();
                userInfo = context.Users.FirstOrDefault(a => a.Email == request.Email);
                var Token = GenerateToken(secret, userInfo);
                var refreshToken = GenerateRefreshToken();
                user = new UserResponse(userInfo, Token, refreshToken.Token);

                response.Result = user;

            }
            catch (Exception)
            {
                response.FailResult("Error occured while creating account");  
            }
            return response;
        }


        public APIResponse<OtpResponse> SendOtp(OtpRequest request)
        {
            var response = new APIResponse<OtpResponse>();
            try
            {
                OtpResponse otp = new OtpResponse();
                var userData = context.Users.FirstOrDefault(u => u.Id == request.UserId);

                if (userData == null)
                {
                    response.FailResult("Incorrect details provided");
                    response.Result = otp;
                    return response;
                }

                UserVerification MobileOTP = context.UserVerifications.FirstOrDefault(v => v.UserId == request.UserId && v.Type == "MOBILE");
                if (MobileOTP == null) MobileOTP = new UserVerification() { UserId = request.UserId, Type = "MOBILE" };


                int OTP = MobileOTP.Id == 0 ? new Random().Next(100000, 999999) : Convert.ToInt32(MobileOTP.Code);

                string Mobile = "+91" + userData.Mobile;
                string Message = "Use OTP " + OTP + " to verify your mobile on Hotebedscache.";
                GlobalUtilities.SendSMS(Mobile, Message, GlobalVariables.SMS_GATEWAY.MSG91);

                MobileOTP.Code = OTP.ToString();
                context.Update<UserVerification>(MobileOTP);
                context.SaveChanges();

                otp.OTPSent = true;
                response.Result = otp;
            }
            catch (Exception)
            {
                response.FailResult("Error occured while sending Otp");
            }
            return response;
        }

        public APIResponse<OtpResponse> VerifyOtp(OtpVerifyRequest request)
        {
            var response = new APIResponse<OtpResponse>();
            try
            {
                OtpResponse otp = new OtpResponse();
                var userData = context.Users.FirstOrDefault(u => u.Id == request.UserId);

                if (userData == null)
                {
                    response.FailResult("Incorrect details provided");
                    response.Result = otp;
                    return response;
                }

                UserVerification MobileOTP = context.UserVerifications.FirstOrDefault(v => v.UserId == request.UserId && v.Type == "MOBILE");
                if (MobileOTP == null)
                {
                    otp.OTPVerified = false;
                    response.FailResult("Invalid Code");
                }
                else
                {
                    if (MobileOTP.Code == request.Code)
                    {
                        List<UserVerification> OldMobileOTPs = context.UserVerifications.Where(v => v.UserId == request.UserId && v.Type == "MOBILE").ToList();
                        foreach (UserVerification OldMobileOTP in OldMobileOTPs) context.Remove<UserVerification>(OldMobileOTP);


                        userData.MobileVerified = true;
                        context.Update<User>(userData);
                        context.SaveChanges();

                        otp.OTPVerified = true;
                        response.Message = "Mobile Number Verified Successfully!";
                        response.Result = otp;
                    }
                    else
                    {
                        otp.OTPVerified = false;
                        response.Result = otp;
                        response.FailResult("Invalid Code");
                    }
                }
            }
            catch (Exception)
            {
                response.FailResult("Error occured while sending Otp");
            }
            return response;
        }

     

        public APIResponse<BaseUser> SaveProfile(BaseUser request)
        {
            var response = new APIResponse<BaseUser>();
            var userInfo = context.Users.FirstOrDefault(a => a.Id == request.UserId);
            if (userInfo != null)
            {
                userInfo.Mobile = request.Mobile;
                userInfo.Role = request.Role; 
                context.SaveChanges();
                response.Result = request;
            }
            else
            {
                response.FailResult("User not found");
            }
            return response;
        }
        public bool UpdateMobileVerified(string mobile)
        {
            var response = new APIResponse<BaseUser>();
            var userInfo = context.Users.FirstOrDefault(a => a.Mobile == mobile);
            if (userInfo != null)
            {
                userInfo.MobileVerified = true;
                context.SaveChanges();
            }
            else
            {
                response.FailResult("User not found");
                return false;
            }
            return true;
        }
        public bool UpdateEmailVerified(string mobile)
        {
            var response = new APIResponse<BaseUser>();
            var userInfo = context.Users.FirstOrDefault(a => a.Mobile == mobile || a.Email == mobile);
            if (userInfo != null)
            {
                userInfo.EmailVerified = true;
                context.SaveChanges();
            }
            else
            {
                response.FailResult("User not found");
                return false;
            }
            return true;
        }
       
        #endregion

    }
}
