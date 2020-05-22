using System;
using Core.Api.Common;
using Hangfire;
using Serilog;

namespace Core.Api.Applications
{
    public class HangfireApplication
    {
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly ILogger _logger;

        public HangfireApplication(IBackgroundJobClient backgroundJobClient,ILogger logger)
        {
            _backgroundJobClient = backgroundJobClient;
            _logger = logger;
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

        public void HangfireTwo()
        {
            _logger.Information("Log Start...........");

            _backgroundJobClient.Enqueue<HangfireService>(x => x.Test("Test Enquence"));
            _backgroundJobClient.Schedule<HangfireService>(x => x.Test("Schedule Delay one Minutes"),TimeSpan.FromMinutes(1));
            _logger.Information("中间商");
            RecurringJob.AddOrUpdate("ss", () => Console.WriteLine("Job Id"),CronType.Minute(2));

            _logger.Information("Log End ..............");
        }
    }
}
