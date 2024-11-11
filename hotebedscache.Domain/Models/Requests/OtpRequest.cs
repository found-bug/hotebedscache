namespace Hotebedscache.Domain.Models.Requests
{
    public class OtpRequest
    {     
        public int? UserId { get; set; }
    }
    public class OtpVerifyRequest : OtpRequest
    {
        public string Code { get; set; }

    }
}
