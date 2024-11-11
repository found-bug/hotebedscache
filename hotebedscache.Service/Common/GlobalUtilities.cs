using Hotebedscache.Domain.Common;

namespace Hotebedscache.Service.Common
{
    public class GlobalUtilities
    {
        public static void SendSMS(string Mobile, string Message, GlobalVariables.SMS_GATEWAY SMS_Gateway)
        {
            if (SMS_Gateway == GlobalVariables.SMS_GATEWAY.MSG91)
            {
                RestSharp.RestClient client = new RestSharp.RestClient("https://api.msg91.com/api/v2/sendsms");
                RestSharp.RestRequest request = new RestSharp.RestRequest(RestSharp.Method.POST);
                request.AddHeader("content-type", "application/json");
                request.AddHeader("authkey", "434085A93xM0TeIBlN672d9bb1P1");
                request.AddParameter("application/json", "{ \"sender\": \"BGBOOK\", \"route\": \"4\", \"country\": \"0\", \"sms\": [ { \"message\": \"" + Message + "\", \"to\": [ \"" + Mobile + "\" ] } ] }", RestSharp.ParameterType.RequestBody);
                RestSharp.IRestResponse response = client.Execute(request);
            }
        }

        public static void LogError(string[] Logs)
        {
            try
            {
                System.IO.File.AppendAllText(@"Maintenance\errorlog-" + DateTime.Today.ToString("ddMMyyyy") + ".txt", "** " + DateTime.Now.ToString("HH:mm:ss") + " **" + Environment.NewLine);
                foreach (string log in Logs)
                {
                    System.IO.File.AppendAllText(@"Maintenance\errorlog-" + DateTime.Today.ToString("ddMMyyyy") + ".txt", "> " + log + Environment.NewLine);
                }
            }
            catch (Exception)
            {
            }

        }
    }
}
