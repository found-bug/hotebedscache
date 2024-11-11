using Quartz;
using Quartz.Impl; 

namespace Hotebedscache.Api.Models
{
    public class JobScheduler
    {
        public static void Start()
        {
            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler().GetAwaiter().GetResult();
            scheduler.Start();

            IJobDetail job = JobBuilder.Create<Worker>().Build();

            ITrigger trigger = TriggerBuilder.Create() 
            .StartNow()
            .WithSimpleSchedule(x => x
            .WithIntervalInHours(24)
            .RepeatForever())
            .Build(); 
            scheduler.ScheduleJob(job, trigger);
        }
    }
}