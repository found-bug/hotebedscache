namespace Hotebedscache.Domain.Common
{
    public static partial class GlobalVariables
    {
        public enum USER_TYPE
        {
            PEER = 0,
            Mentor = 1,
            Admin = 2,
            ThirdParty=3
        }
          
        public enum PAYMENT_GATEWAY
        {
            PAYPAL = 1,
            RAZORPAY = 2,
            STRIPE = 3
        }

        public enum CLOUD_STORAGE
        {
            GOOGLE_DRIVE = 1,
        }

        public enum SMS_GATEWAY
        {
            MSG91 = 1
        } 

        public static Dictionary<string, string> CalendarEventTypes = new Dictionary<string, string>()
        {
            {"LM", "LEGAL MATTER"},
            {"LM_HD", "LEGAL MATTER HEARING DATE"},
        };
         

        public static string GoogleDrive_client_id = "846842763184-klg5ainvvjtod0i2ea5j97fjjcle6n10.apps.googleusercontent.com";
        public static string GoogleDrive_client_secret = "MwwGt-ZZ-KHl9KJBOFFU1GOl"; 
    }
}
