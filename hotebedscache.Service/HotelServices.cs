using Hotebedscache.Service.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Hotebedscache.Service
{
    public class HotelServices : IHotelServices
    {
        public HotelServices()
        {
            
        }

        protected string baseurl = "https://aif2.hotelbeds.com/aif2-pub-ws/files/full";
        protected string apikey = "85e615f6ba142ca87bc3d852d3481665";
        protected string secretkey = "2d1087e6bd";
        public async Task<string> callapi()
        {
            string signature;
            using (var sha = SHA256.Create())
            {

                long ts = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds / 1000;
                var computedHash = sha.ComputeHash(Encoding.UTF8.GetBytes(apikey + secretkey + ts));
                signature = BitConverter.ToString(computedHash).Replace("-", "");
                string response = "";
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                string path = @"C:/Users/HP/Downloads/Hoteldata";
                using (var client = new WebClient())
                {
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12; 
                    client.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip");
                    client.Headers.Add("Api-Key", apikey);
                    client.Headers["X-Signature"] = signature;
                    client.DownloadFile(baseurl, path + "File.zip");
                }
                return response;
            }
        }
        }
}
