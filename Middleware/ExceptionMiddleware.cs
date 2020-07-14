using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Api.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate next;
        private IHostingEnvironment environment;

        public ExceptionMiddleware(RequestDelegate next, IHostingEnvironment environment)
        {
            this.next = next;
            this.environment = environment;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next.Invoke(context);
                var features = context.Features;
                Console.WriteLine(JsonConvert.SerializeObject(features));
            }
            catch (Exception e)
            {
                await HandleException(context, e);
            }
        }

        private async Task HandleException(HttpContext context, Exception e)
        {
            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json;charset=utf-8;";
            string error = "";

            if (environment.IsDevelopment())
            {
                var json = new { message = e.Message };
                error = JsonConvert.SerializeObject(json);
            }
            else
                error = "抱歉，出错了";

            await context.Response.WriteAsync(error);
            //await context.Response.WriteAsync(error);
        }
    }
}
