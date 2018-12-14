using System;
using System.Collections.Generic;
using System.Text;

namespace Sikiro.SMS.Api.Model.Extensions
{
    /// <summary>
    /// API 返回JSON字符串
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ApiResult<T>
    {
        /// <summary>
        /// 信息
        /// </summary>
        public string message { get; set; }
        /// <summary>
        /// 状态码
        /// </summary>
        public int code { get; set; }
        /// <summary>
        /// 数据集
        /// </summary>
        public T data { get; set; }
    }
}
