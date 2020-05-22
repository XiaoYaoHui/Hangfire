using Core.Api.Applications;
using Microsoft.AspNetCore.Mvc;

namespace Core.Api.Controllers
{
    [Route("api/[controller]")]
    public class HangfiresController : Controller
    {
        private readonly HangfireApplication _hangfire;

        public HangfiresController(HangfireApplication hangfire)
        {
            _hangfire = hangfire;
        }

        /// <summary>
        /// 创建调度任务
        /// </summary>
        [HttpGet]
        [Route("/Hangfire")]
        public void CreateHangfire()
        {
            _hangfire.HangfireTwo();
        }
    }
}
