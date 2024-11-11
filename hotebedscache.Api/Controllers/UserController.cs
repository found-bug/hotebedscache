using Microsoft.AspNetCore.Authorization; 
using Microsoft.AspNetCore.Mvc; 
using Hotebedscache.Domain.Models.Requests;
using Hotebedscache.Domain.Models.Responses;
using Hotebedscache.Service.Interfaces;
using Hotebedscache.Service.Helper; 

namespace Hotebedscache.Api.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IConfiguration _configuration;
        public UserController(IAccountService accountService, IConfiguration configuration)
        {
            _configuration = configuration;
            _accountService = accountService;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(APIResponse<UserResponse>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(APIResponse<UserResponse>))]
        public IActionResult Login(UserRequest request)
        {
            if (request != null)
            {
                string secret = _configuration.GetValue<string>("AppSettings:Secret");
                var response = _accountService.Login(request, secret);
                if (response.Status)
                {
                    response.Message = "Login Successfully";
                    return Ok(response);
                }
                return BadRequest(response);
            }
            return BadRequest(new APIResponse<UserResponse> { Message = "Invalid data provided", Status = false });
        }

        [AllowAnonymous]
        [HttpPost("refresh-token")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(APIResponse<UserResponse>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(APIResponse<UserResponse>))]
        public IActionResult RefreshToken(RefreshTokenRequest request)
        {
            if (request != null && !string.IsNullOrEmpty(request.Token))
            {
                string secret = _configuration.GetValue<string>("AppSettings:Secret");
                var response = _accountService.RefreshToken(request.Token, secret);
                if (response.Status)
                {
                    response.Message = "Refresh token generated successfully";
                    return Ok(response);
                }
                return BadRequest(response);
            }
            return BadRequest(new APIResponse<UserResponse> { Message = "Invalid data provided", Status = false });
        }


        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(APIResponse<UserResponse>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(APIResponse<UserResponse>))]
        public async Task<IActionResult> Register(RegistrationRequest request)
        {
            if (request != null && !string.IsNullOrEmpty(request.Email) && !string.IsNullOrEmpty(request.FirstName) && !string.IsNullOrEmpty(request.LastName))
            {
                string secret = _configuration.GetValue<string>("AppSettings:Secret");
                var response = await _accountService.Register(request, secret);

                if (response.Status)
                { 
                    return Ok(response);
                }
                return BadRequest(response);
            }
            return BadRequest(new APIResponse<UserResponse> { Message = "Invalid data provided", Status = false });
        }
        [HttpGet("VerifyEmail/{Email}")]
        public IActionResult VerifyEmail(string Email)
        {
            if (Email != null)
            { 
                var response = _accountService.UpdateEmailVerified(Email); 
                return Ok(response);
            }
            return BadRequest(false);
        }
        /// <summary>
        /// Sending and Resending SMS
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("mobile/Otp")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(APIResponse<OtpResponse>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(APIResponse<OtpResponse>))]
        public IActionResult Otp(OtpRequest request)
        {
            if (request != null && request.UserId != null && request.UserId != 0)
            {
                var response = _accountService.SendOtp(request);
                if (response.Status)
                {
                    response.Message = "OTP sent to your mobile";
                    return Ok(response);
                }
                return BadRequest(response);
            }
            return BadRequest(new APIResponse<OtpResponse> { Message = "Invalid data provided", Status = false });
        }

        /// <summary>
        /// Verify OTP
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("mobile/verify")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(APIResponse<OtpResponse>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(APIResponse<OtpResponse>))]
        public IActionResult verify(OtpVerifyRequest request)
        {
            if (request != null && request.UserId != null && !string.IsNullOrEmpty(request.Code)
                                && request.UserId != 0)
            {
                var response = _accountService.VerifyOtp(request);
                if (response.Status)
                {
                    return Ok(response);
                }
                return BadRequest(response);
            }
            return BadRequest(new APIResponse<OtpResponse> { Message = "Invalid data provided", Status = false });
        }
     
        [HttpPost("profile")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(APIResponse<BaseUser>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(APIResponse<BaseUser>))]
        public IActionResult UpdateProfile(BaseUser request)
        {
            if (request.UserId != 0)
            {
                APIResponse<BaseUser> response = _accountService.SaveProfile(request);
                if (response.Status)
                {
                    response.Message = "User profile updated successfully.";
                    return Ok(response);
                }
                return BadRequest(response);
            }
            return BadRequest(new APIResponse<BaseUser> { Message = "Invalid data provided", Status = false });
        } 
    }
}
