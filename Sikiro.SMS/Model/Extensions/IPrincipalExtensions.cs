using System;
using System.Security.Claims;

namespace Sikiro.SMS.Api.Model.Extensions
{
    public static class IPrincipalExtensions
    {
        /// <summary>
        /// 组织ID
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static int projectId(this ClaimsPrincipal user) => Convert.ToInt32(user.FindFirst("project_id").Value);

        ///// <summary>
        ///// 用户ID
        ///// </summary>
        ///// <param name="user"></param>
        ///// <returns></returns>
        //public static int userId(this ClaimsPrincipal user) => Convert.ToInt32(user.FindFirst("app_id").Value);

        ///// <summary>
        ///// 用户编号
        ///// </summary>
        ///// <param name="user"></param>
        ///// <returns></returns>
        //public static string userNo(this ClaimsPrincipal user) => user.FindFirst("app_secret").Value;      
    }
}
