using System;
using System.IO;
using Core.Api.Applications;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.Dashboard.BasicAuthorization;
using Hangfire.MySql.Core;
using Core.Api.Middleware;
using Core.Api.Filter;

namespace Core.Api
{
    /// <summary>
    /// --
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// 启动
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }
        //private readonly ILogger _logger;

        /// <summary>
        ///This method gets called by the runtime. Use this method to add services to the container. 
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {

            #region Swagger

            services.AddSwaggerGen(swagger =>
            {
                swagger.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "My Api",
                    Version = "v1"
                });
                var basePath =  AppContext.BaseDirectory;
                var xmlPath = Path.Combine(basePath, "Core.Api.xml");
                swagger.IncludeXmlComments(xmlPath, true);
            });

            #endregion

            #region Hangfire

            //var mysqlHangFire = Configuration.GetConnectionString("HangfireConnection");
            //services.AddHangfire(x => x.UseStorage(new MySqlStorage(mysqlHangFire,new MySqlStorageOptions
            //{
            //    //TablePrefix = MeowvBlogConsts.DbTablePrefix + "hangfire",                           //数据表前缀
            //    TransactionIsolationLevel = System.Data.IsolationLevel.ReadCommitted,       //  事务隔离级别。默认值为读提交
            //    TransactionTimeout = TimeSpan.FromMinutes(1),                                               // 事务超时。默认为1分钟
            //    QueuePollInterval = TimeSpan.FromSeconds(15),                                                // 作业队列轮询间隔。默认值为15秒
            //    JobExpirationCheckInterval = TimeSpan.FromHours(1),                                     //  作业过期检查间隔（管理过期记录）。默认为1小时
            //    CountersAggregateInterval = TimeSpan.FromMinutes(5),                                // 间隔到聚合计数器。默认为5分钟
            //    PrepareSchemaIfNecessary = true,                                                                        //如果设置为true，则创建数据库表。默认值为true
            //    DashboardJobListLimit = 50000,                                                                           // 仪表板作业列表上限。默认值为50000
            //})).UseSerilogLogProvider());

            //services.AddHangfireServer(n => n.CancellationCheckInterval = TimeSpan.FromSeconds(5));
            //services.AddScoped<HangfireApplication>();
            //services.AddScoped<HangfireService>();
            #endregion

            services.AddScoped<IStudentRepository, StudentRepository>();

            services.AddScoped<GlobalExceptionFilter>();    

            services.AddControllers();
        }

        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app/*, IBackgroundJobClient backgroundJobs*/,IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            //app.UseHttpsRedirection();

            #region Swagger
            app.UseSwagger();
            app.UseSwaggerUI(s =>
            {
                //swagger/<document-name>/swagger.json  where是对的调用中提供的SwaggerDoc参数中的name
                s.SwaggerEndpoint("/swagger/v1/swagger.json", "My Web API V1");
                s.RoutePrefix = "";
                //路径配置，设置为空，表示直接访问该文件，表示直接在根域名（localhost:8001）访问该文件
                //这个时候去launchSettings.json中把"launchUrl": "swagger/index.html"去掉， 然后直接访问localhost:8001/index.html即可  
            });
            #endregion

            //倾向于不希望对静态文件进行请求日志记录
            //app.UseSerilogRequestLogging();

            app.UseMiddleware<ExceptionMiddleware>();


            app.UseRouting();

            #region 跨域
            app.UseCors(config =>
            {
                config.AllowAnyOrigin();
                config.AllowAnyMethod();
                config.AllowAnyHeader();
            });
            #endregion

            //app.UseAuthentication();

            app.UseAuthorization();


            #region Hangfire

            //Map to the "/hangfire"，DashboardOptions ， 多个Dashboard，使用不同的storage
            //app.UseHangfireDashboard("/hangfire", options:new DashboardOptions
            //{
            //    //AppPath = "http://your-app.net",    //Back to site link，返回的链接地址
            //    IsReadOnlyFunc = (DashboardContext context) => true,  //ReadOnly View
            //    //Authorization = new IDashboardAuthorizationFilter[]
            //    //{
            //    //    new MyAuthorizationFilter()
            //    //},
            //    Authorization = new[]
            //    {
            //        new BasicAuthAuthorizationFilter(new BasicAuthAuthorizationFilterOptions
            //        {
            //            RequireSsl = false,
            //            SslRedirect = false,
            //            LoginCaseSensitive = true,
            //            Users = new []
            //            {
            //                new BasicAuthAuthorizationUser
            //                {
            //                    Login = Configuration.GetSection("Hangfire:Username").Get<string>(),     // AppSettings.Hangfire.Login,
            //                    PasswordClear = Configuration.GetSection("Hangfire:Password").Get<string>(), // AppSettings.Hangfire.Password
            //                }
            //            }
            //        })
            //    },
            //    DashboardTitle = "任务调度中心",
            //});

            //app.UseHangfireServer(new BackgroundJobServerOptions
            //{
            //    ServerName = $"{Environment.MachineName}-{Guid.NewGuid().ToString()}",
            //    Queues = new string[]{"default","quece001"},    //队列名称参数必须仅包含小写字母，数字和下划线字符
            //    WorkerCount = Environment.ProcessorCount * 5, //核心数，最大限制20 //并发任务数
            //    SchedulePollingInterval = TimeSpan.FromMinutes(1)   //轮询时间间隔  ，设置成1分钟
            //});
            //backgroundJobs.Enqueue(() => Console.WriteLine("Start up"));
            #endregion

            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapControllerRoute(
                //    name: default, 
                //    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapControllers();
            });
        }
    }
}
