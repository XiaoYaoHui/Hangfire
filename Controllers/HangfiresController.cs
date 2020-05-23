using Core.Api.Applications;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Core.Api.Controllers
{
    [Route("api/[controller]")]
    public class HangfiresController : Controller
    {
        private readonly HangfireApplication _application;

        public HangfiresController(HangfireApplication application)
        {
            _application = application;
        }
        /// <summary>
        /// 创建调度任务
        /// </summary>
        [HttpGet]
        public void CreateHangfire()
        {
            Log.Information("CreateHangfire");
            _application.HangfireTwo();
        }
    }
}
