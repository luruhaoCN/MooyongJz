using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace MooyongCommon
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate next;
        private IHostEnvironment environment;

        public ExceptionMiddleware(RequestDelegate next, IHostEnvironment environment)
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
            }
            catch (Exception e)
            {
                await HandleException(context, e);
            }
        }

        private async Task HandleException(HttpContext context, Exception e)
        {
            context.Response.StatusCode = 500;
            context.Response.ContentType = "text/json;charset=utf-8;";
            string error;
            if (environment.IsDevelopment())
            {
                var json = new ApiResult().SetFailedResult("0",e.Message);
                error = JsonConvert.SerializeObject(json);
            }
            else
            {
                var json = new ApiResult().SetFailedResult("0", "抱歉，出错了！");
                error = JsonConvert.SerializeObject(json);
            }
            await context.Response.WriteAsync(error);
        }
    }
}
