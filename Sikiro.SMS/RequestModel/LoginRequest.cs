namespace Sikiro.SMS.Api
{
    /// <summary>
    /// 登录请求参数
    /// </summary>
    public class LoginRequest
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string username { get; set; }
        /// <summary>
        /// 用户密码
        /// </summary>
        public string password { get; set; }
    }
}
