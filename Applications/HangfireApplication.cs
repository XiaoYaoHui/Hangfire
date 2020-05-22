using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;

namespace Core.Api.Applications
{
    public class HangfireApplication
    {
        private readonly IBackgroundJobClient _backgroundJobClient;

        public HangfireApplication(IBackgroundJobClient backgroundJobClient)
        {
            _backgroundJobClient = backgroundJobClient;
        }

        public void HangfireTest()
        {
            //Always dispose background server
            var hangfire = new BackgroundJobServer();
            hangfire.Dispose();

            _backgroundJobClient.Enqueue<HangfireService>(x => x.Test("Test Hangfire"));

            _backgroundJobClient.Enqueue<HangfireService>(h => h.Test("Test"));
            _backgroundJobClient.Enqueue(() => Console.WriteLine("Hello World for Hang-fire"));
            _backgroundJobClient.Schedule(() => Console.WriteLine("Schedule is started"), TimeSpan.FromMinutes(1));
            RecurringJob.AddOrUpdate("First RecurrringJob", () => Console.WriteLine("RecurringJob is running"), cronExpression: Cron.MinuteInterval(interval: 2));
            RecurringJob.RemoveIfExists("Test remove RecurringJob");
            RecurringJob.Trigger("First RecurringJob");
        }
    }
}
