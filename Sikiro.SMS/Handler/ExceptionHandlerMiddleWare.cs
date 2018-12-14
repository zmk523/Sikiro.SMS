using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Sikiro.SMS.Api.Model.Extensions;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Sikiro.SMS.Api.Handler
{
    /// <summary>
    /// 全局异常捕捉
    /// </summary>
    public class ExceptionHandlerMiddleWare
    {
        /// <summary>
        /// 日志操作
        /// </summary>
        private readonly ILogger logger;
        /// <summary>
        /// 请求处理任务
        /// </summary>
        private readonly RequestDelegate next;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="next"></param>
        /// <param name="loggerFactory"></param>
        public ExceptionHandlerMiddleWare(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            this.next = next;
            logger = loggerFactory.CreateLogger<ExceptionHandlerMiddleWare>();
        }
        /// <summary>
        /// 调用
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context)
        {
            var request = context.Request;
            var log = new NlogExtensions()
            {
                TraceIdentifier = context.TraceIdentifier,
                Method = request.Method,
                Ip = context.Connection.RemoteIpAddress.ToString(),
                Time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                UserAgent = request.Headers["User-Agent"],
                Url = request.Host.Value + request.Path.Value
            };


            try
            {
                //请求是post类型 排除上传文件的流
                if (log.Method == "POST" && !request.HasFormContentType)
                {
                    GetFromBody(request, log);
                }
                else
                {
                    log.Url = log.Url + request.QueryString.Value;
                }

                await next(context);
            }
            catch (Exception ex)
            {
                int statusCode = 500;
                var msg = ex.Message;
                if (ex.HResult > 0)
                {
                    statusCode = ex.HResult;
                }
                string exceptionMsg = "";
                if (statusCode <= 500)
                {
                    //对异常进行加密输出
                    Encoding encodingUTF8 = Encoding.UTF8;
                    byte[] bytes = encodingUTF8.GetBytes(ex.ToString());
                    context.Response.Headers.Add("Error", Convert.ToBase64String(bytes));
                    msg = "系统异常,请联系管理员";
                    exceptionMsg = ex.ToString();
                }
                await HandleExceptionAsync(context, statusCode, exceptionMsg, msg, log);
            }
            finally
            {
                var statusCode = context.Response.StatusCode;
                var msg = "";
                switch (statusCode)
                {
                    case 401:
                        msg = "未授权";
                        break;
                    case 404:
                        msg = "未找到服务";
                        break;
                    case 502:
                        msg = "请求错误";
                        break;
                    case 302:
                    case 200:
                        break;
                    default:
                        msg = "未知错误";
                        break;
                }
                if (!string.IsNullOrWhiteSpace(msg))
                {
                    await HandleExceptionAsync(context, statusCode, "", msg, log);
                }

                if (log.StatusCode == 0)
                {
                    log.UserId = GetUserInfoByType("projectId", context);
                    //log.UserName = GetUserInfoByType("userName", context);
                    log.StatusCode = statusCode;
                    log.Result = "成功";
                    log.Levels = LogLevel.Information.ToString();
                    logger.LogInformation(GetLogJson(log));
                }
            }
        }



        /// <summary>
        /// 结果处理
        /// </summary>
        /// <param name="context"></param>
        /// <param name="statusCode"></param>
        /// <param name="exceptionMsg"></param>
        /// <param name="msg"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        private Task HandleExceptionAsync(HttpContext context, int statusCode, string exceptionMsg, string msg, NlogExtensions log)
        {
            var data = new ApiResult<object>
            {
                code = statusCode,
                message = msg
            };
            var result = JsonConvert.SerializeObject(data);
            context.Response.ContentType = "application/json;charset=utf-8";
            context.Response.StatusCode = 200;

            log.Result = msg;
            log.StatusCode = statusCode;
            log.UserId = GetUserInfoByType("projectId", context);
            //log.UserName = GetUserInfoByType("userName", context);
            if (!string.IsNullOrEmpty(exceptionMsg))
            {
                log.Levels = LogLevel.Error.ToString();
                log.Exception = exceptionMsg;
                logger.LogError(GetLogJson(log));
            }
            else
            {
                log.Levels = LogLevel.Information.ToString();
                logger.LogInformation(GetLogJson(log));
            }
            return context.Response.WriteAsync(result);
        }

        /// <summary>
        /// 获取fromBody内容
        /// </summary>
        /// <param name="request"></param>
        /// <param name="log"></param>
        private static void GetFromBody(HttpRequest request, NlogExtensions log)
        {
            var injectedRequestStream = new MemoryStream();
            using (var bodyReader = new StreamReader(request.Body))
            {
                var bodyAsText = bodyReader.ReadToEnd();
                if (string.IsNullOrWhiteSpace(bodyAsText) == false)
                {
                    log.FromBody = bodyAsText;
                }

                //读取完body的内容后 需要把内容重新写回去 不然所有controllers获取不到参数
                var bytesToWrite = Encoding.UTF8.GetBytes(bodyAsText);
                injectedRequestStream.Write(bytesToWrite, 0, bytesToWrite.Length);
                injectedRequestStream.Seek(0, SeekOrigin.Begin);
                request.Body = injectedRequestStream;
            }
        }

        /// <summary>
        /// 获取json格式的log
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        private static string GetLogJson(NlogExtensions log)
        {
            return JsonConvert.SerializeObject(log, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="type"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        private static string GetUserInfoByType(string type, HttpContext context)
        {
            var claims = context.User.FindFirst(type);
            if (claims != null)
            {
                return claims.Value;
            }
            return null;
        }
    }

    /// <summary>
    /// 错误处理扩展类
    /// </summary>
    public static class ErrorHandlingExtensions
    {
        /// <summary>
        /// 引入全局异常类
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseErrorHandling(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlerMiddleWare>();
        }
    }
}
