using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Api.Filter;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Core.Api.Controllers
{
    /// <summary>
    /// 异常
    /// </summary>
    [Route("api/[controller]")]
    public class ExceptionController : Controller
    {
        /// <summary>
        /// 捕获异常
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Get")]
        [ServiceFilter(typeof(GlobalExceptionFilter))]
        public IActionResult Index()
        {
            int a = 0, b = 5;
            var result = b / a;
            return Ok(result);
        }
    }
}
