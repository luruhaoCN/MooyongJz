using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;

namespace MooyongCommon
{
    public class GlobalExceptionFilter : Attribute, IExceptionFilter
    {
        private readonly IHostEnvironment _hostingEnvironment;
        private readonly IModelMetadataProvider _modelMetadataProvider;

        public GlobalExceptionFilter(
            IHostEnvironment hostingEnvironment,
            IModelMetadataProvider modelMetadataProvider)
        {
            _hostingEnvironment = hostingEnvironment;
            _modelMetadataProvider = modelMetadataProvider;
        }

        /// <summary>
        /// 发生异常进入
        /// </summary>
        /// <param name="context"></param>
        public void OnException(ExceptionContext context)
        {
            ContentResult result = new ContentResult
            {
                StatusCode = 500,
                ContentType = "text/json;charset=utf-8;"
            };

            if (_hostingEnvironment.IsDevelopment())
            {
                var json = new ApiResult().SetFailedResult("0", context.Exception.Message);
                result.Content = JsonConvert.SerializeObject(json);
            }
            else
            {
                var json = new ApiResult().SetFailedResult("0", "抱歉，出错了！");
                result.Content = JsonConvert.SerializeObject(json);
            }
            ApiLogger.Info(result.Content);
            context.Result = result;
            context.ExceptionHandled = true;
        }
    }
}
