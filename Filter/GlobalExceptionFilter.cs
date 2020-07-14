using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Api.Filter
{
    public class GlobalExceptionFilter : Attribute,IExceptionFilter
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IModelMetadataProvider _modelMetadataProvider;

        public GlobalExceptionFilter(IHostingEnvironment hostingEnvironment,
            IModelMetadataProvider modelMetadataProvider)
        {
            _hostingEnvironment = hostingEnvironment;
            _modelMetadataProvider = modelMetadataProvider;
        }

        public async void OnException(ExceptionContext context)
        {
            ContentResult result = new ContentResult
            {
                StatusCode = 500,
                ContentType = "text/json;charset=utf-8;"
            };

            if (_hostingEnvironment.IsDevelopment())
            {
                var json = new { message = context.Exception.Message };
                result.Content = JsonConvert.SerializeObject(json);
            }
            else
            {
                result.Content = "抱歉，出错了";
            }
            context.Result = result;
            context.ExceptionHandled = true;
        }


        /// <summary>
        /// 发生异常进入
        /// </summary>
        /// <param name="context"></param>
    }
}
