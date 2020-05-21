using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using ILogger = Serilog.ILogger;

namespace Core.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Debug()//��С�������λ��Debug�����
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)//��Microsoftǰ׺����־����С�������ĳ�Information
                    .MinimumLevel.Override("System",LogEventLevel.Information)
                    .Enrich.FromLogContext()
                    .WriteTo.Console(restrictedToMinimumLevel:LogEventLevel.Information, outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:j} {NewLine}{Exception}")
                    //.WriteTo.File(AppDomain.CurrentDomain.BaseDirectory + "//LogException/log-.txt",LogEventLevel.Error, rollingInterval: RollingInterval.Day)
                    //.WriteTo.RollingFile(AppDomain.CurrentDomain.BaseDirectory + "//Log/log.txt",LogEventLevel.Information)
                    .CreateLogger();

                Log.Information("Start up");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application start-up failed");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
                //ʹ��Serilog�������ļ�
                //    .UseSerilog((context, configure) =>
                //{
                //    configure.ReadFrom.Configuration(context.Configuration);
                //});
        }
}
