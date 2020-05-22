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

namespace Core.Api
{
    /// <summary>
    /// --
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// ����
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        public Startup(IConfiguration configuration/*,ILogger logger*/)
        {
            //_logger = logger;
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
            
            var mysqlHangFire = Configuration.GetConnectionString("HangfireConnection");
            services.AddHangfire(x => x.UseStorage(new MySqlStorage(mysqlHangFire,new MySqlStorageOptions
            {
                //TablePrefix = MeowvBlogConsts.DbTablePrefix + "hangfire",                           //���ݱ�ǰ׺
                TransactionIsolationLevel = System.Data.IsolationLevel.ReadCommitted,       //  ������뼶��Ĭ��ֵΪ���ύ
                TransactionTimeout = TimeSpan.FromMinutes(1),                                               // ����ʱ��Ĭ��Ϊ1����
                QueuePollInterval = TimeSpan.FromSeconds(15),                                                // ��ҵ������ѯ�����Ĭ��ֵΪ15��
                JobExpirationCheckInterval = TimeSpan.FromHours(1),                                     //  ��ҵ���ڼ������������ڼ�¼����Ĭ��Ϊ1Сʱ
                CountersAggregateInterval = TimeSpan.FromMinutes(5),                                // ������ۺϼ�������Ĭ��Ϊ5����
                PrepareSchemaIfNecessary = true,                                                                        //�������Ϊtrue���򴴽����ݿ��Ĭ��ֵΪtrue
                DashboardJobListLimit = 50000,                                                                           // �Ǳ����ҵ�б����ޡ�Ĭ��ֵΪ50000
            })).UseSerilogLogProvider());

            //services.AddHangfireServer(n => n.CancellationCheckInterval = TimeSpan.FromSeconds(5));

            #endregion

            services.AddScoped<IStudentRepository, StudentRepository>();
            services.AddScoped<HangfireApplication>();
            services.AddControllers();
        }

        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IBackgroundJobClient backgroundJobs,IWebHostEnvironment env)
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
                //swagger/<document-name>/swagger.json  where�ǶԵĵ������ṩ��SwaggerDoc�����е�name
                s.SwaggerEndpoint("/swagger/v1/swagger.json", "My Web API V1");
                s.RoutePrefix = "";
                //·�����ã�����Ϊ�գ���ʾֱ�ӷ��ʸ��ļ�����ʾֱ���ڸ�������localhost:8001�����ʸ��ļ�
                //���ʱ��ȥlaunchSettings.json�а�"launchUrl": "swagger/index.html"ȥ���� Ȼ��ֱ�ӷ���localhost:8001/index.html����  
            });
            #endregion

            //�����ڲ�ϣ���Ծ�̬�ļ�����������־��¼
            app.UseSerilogRequestLogging();

            app.UseRouting();

            #region ����
            app.UseCors(config =>
            {
                config.AllowAnyOrigin();
                config.AllowAnyMethod();
                config.AllowAnyHeader();
            });
            #endregion

            app.UseAuthentication();

            app.UseAuthorization();

            #region Hangfire
            
            //Map to the "/hangfire"��DashboardOptions �� ���Dashboard��ʹ�ò�ͬ��storage
            app.UseHangfireDashboard("/hangfire", options:new DashboardOptions
            {
                //AppPath = "http://your-app.net",    //Back to site link�����ص����ӵ�ַ
                IsReadOnlyFunc = (DashboardContext context) => true,  //ReadOnly View
                //Authorization = new IDashboardAuthorizationFilter[]
                //{
                //    new MyAuthorizationFilter()
                //},
                Authorization = new[]
                {
                    new BasicAuthAuthorizationFilter(new BasicAuthAuthorizationFilterOptions
                    {
                        RequireSsl = false,
                        SslRedirect = false,
                        LoginCaseSensitive = true,
                        Users = new []
                        {
                            new BasicAuthAuthorizationUser
                            {
                                Login = Configuration.GetSection("Hangfire:Username").Get<string>(),     // AppSettings.Hangfire.Login,
                                PasswordClear = Configuration.GetSection("Hangfire:Password").Get<string>(), // AppSettings.Hangfire.Password
                            }
                        }
                    })
                },
                DashboardTitle = "�����������",
            });

            app.UseHangfireServer(new BackgroundJobServerOptions
            {
                ServerName = $"{Environment.MachineName}-{Guid.NewGuid().ToString()}",
                Queues = new string[]{"default","quece001"},    //�������Ʋ������������Сд��ĸ�����ֺ��»����ַ�
                WorkerCount = Environment.ProcessorCount * 5, //���������������20 //����������
                SchedulePollingInterval = TimeSpan.FromMinutes(1)   //��ѯʱ����  �����ó�1����
            });
            backgroundJobs.Enqueue(() => Console.WriteLine("Start up"));
            #endregion

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
