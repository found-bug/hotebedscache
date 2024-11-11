using Quartz;
namespace Hotebedscache.Api.Models
{
    public class Worker : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            await Console.Out.WriteLineAsync("ListingJob is executing.");

            string filePath = AppDomain.CurrentDomain.BaseDirectory;

            await GetAPIData();
        }


        #region GetAPIData
        private async Task GetAPIData()
        {

            try
            {
            }
            catch (Exception ex) { throw ex; }
        } 
        #endregion 

    }

}