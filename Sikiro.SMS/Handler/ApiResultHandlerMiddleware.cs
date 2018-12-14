using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Sikiro.SMS.Api.Model.Extensions;

namespace Sikiro.SMS.Api.Handler
{
    /// <summary>
    /// 返回标准统一json格式
    /// </summary>
    public class ApiResultHandlerMiddleware : ActionFilterAttribute
    {
        /// <summary>
        /// 重写OnResultExecuting方法
        /// </summary>
        /// <param name="context"></param>
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            if (context.Result is ObjectResult)
            {
                var objectResult = context.Result as ObjectResult;
                if (objectResult.Value == null)
                {
                    context.Result = new ObjectResult(new ApiResult<string> { code = 404, message = "未找到资源" });
                }
                else
                {
                    context.Result = new ObjectResult(new ApiResult<object> { code = 0, data = objectResult.Value, message = "成功" });
                }
            }//TODO context.Result 不处理 
            else if (context.Result is EmptyResult)
            {
                context.Result = new ObjectResult(new ApiResult<object> { code = 0, message = "成功" });
            }
        }
    }
}
