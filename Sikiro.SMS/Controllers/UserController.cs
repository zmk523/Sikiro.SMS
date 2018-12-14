using Kj.Com.Exceptionless;
using Kj.Com.JWT;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Sikiro.SMS.Api.Controllers
{
    /// <summary>
    /// 用户管理
    /// </summary>
    [Produces("application/json")]
    [Authorize("Bearer")]
    [EnableCors("Any")]
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        private JwtRequirement _requirement;  //自定义策略参数

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="requirement"></param>
        public UserController(JwtRequirement requirement) 
        {
            _requirement = requirement;
        }

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="request">请求参数(账号、密码)</param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("login")]
        public object Login([FromBody]LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.username) || string.IsNullOrWhiteSpace(request.password))
                throw new BusinessException("用户名或密码必填", ExceptionCode.ParameterError);
            if (request.username != "admin" || request.password != "671abd11ef0a46ca8c1467e6c028c015") throw new BusinessException("登录失败,无效的账号！", ExceptionCode.DataError);

            //自定义token过期时间
            double expireTotalSeconds = 60 * 24 * 365 * 5;//model.tokenExpireTime > 0 ? model.tokenExpireTime * 60 : _requirement.Expiration.TotalSeconds;
            _requirement.Expiration = TimeSpan.FromSeconds(expireTotalSeconds);

            //如果是基于用户的授权策略，这里要添加用户;如果是基于角色的授权策略，这里要添加角色
            var claims = new Claim[] {
                new Claim(ClaimTypes.Expiration, DateTime.Now.AddSeconds(expireTotalSeconds).ToString())//设置过期时间
            };
            //用户标识
            var identity = new ClaimsIdentity(JwtBearerDefaults.AuthenticationScheme);
            identity.AddClaims(claims);

            var token = JwtToken.BuildJwtToken(claims, _requirement);

            return new
            {
                token
            };
        }

        /// <summary>
        /// 创建账号
        /// </summary>
        /// <returns></returns>
        [HttpPost("create/account")]
        public object CreateAccount()
        {
            return
                 new
                 {
                     code = DateTime.Now.ToString("yyyyMMddHHmmssfff"),
                     key = Guid.NewGuid().ToString("N")
                 };
        }


        ///// <summary>
        ///// 获取用户列表
        ///// </summary>
        ///// <param name="projectId">组织架构ID</param>
        ///// <returns></returns>
        //[HttpGet("get/list/{projectId}")]
        //public async Task<List<M_Sys_User>> GetOrgUserList(int projectId)
        //{
        //    var list = await new BLL_SysUser(_userService, _menuService, _mapper, _logService, User, _apiUrls, Ip).GetOrgUserList(projectId);
        //    return list;
        //}

        ///// <summary>
        ///// 添加用户
        ///// </summary>
        ///// <param name="sysUser">用户对象实体</param>
        ///// <returns></returns>
        //[HttpPost("add")]
        //[IdempotentFilter]
        //public async Task<int> Add([FromBody]M_Sys_User sysUser)
        //{
        //    return await new BLL_SysUser(_userService, _menuService, _mapper, _logService, User, _apiUrls, Ip).Add(sysUser);
        //}

        /////// <summary>
        /////// 修改用户密码
        /////// </summary>
        /////// <param name="request">请求参数对象</param>
        /////// <returns></returns>
        ////[HttpPost("updatepwd")]
        ////[IdempotentFilter]
        ////public async Task<int> UpdatePwd([FromBody]UpdatePwdRequest request)
        ////{
        ////    return await new BLL_User(_userService, _menuService, _mapper, _logService, User, _apiUrls, Ip).UpdatePwd(request.newPwd, request.oldPwd);
        ////} 

        ///// <summary>
        ///// 修改用户
        ///// </summary>
        ///// <param name="sysUser">用户对象实体</param>
        ///// <returns></returns>
        //[HttpPost("update")]
        //[IdempotentFilter]
        //public async Task<int> Update([FromBody]M_Sys_User sysUser)
        //{
        //    return await new BLL_SysUser(_userService, _menuService, _mapper, _logService, User, _apiUrls, Ip).Update(sysUser);
        //}

        ///// <summary>
        ///// 删除用户
        ///// </summary>
        ///// <param name="id">用户id</param>
        ///// <returns></returns>
        //[HttpGet("delete/{id}")]
        //[IdempotentFilter]
        //public async Task<int> Delete(int id)
        //{
        //    return await new BLL_SysUser(_userService, _menuService, _mapper, _logService, User, _apiUrls, Ip).Delete(id);
        //}

        /// <summary>
        /// 注销用户
        /// </summary>
        /// <returns></returns>
        [HttpGet("logout")]
        public async Task<bool> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return true;
        }

        /// <summary>
        /// 未授权
        /// </summary>
        [HttpGet("denied")]
        [AllowAnonymous]
        public void Denied()
        {
            throw new BusinessException("您无权限访问该接口", ExceptionCode.DataUnsupported);
        }

        ///// <summary>
        ///// 修改用户角色
        ///// </summary>
        ///// <param name="roleId">角色ID</param>
        ///// <param name="userId">用户ID</param>
        ///// <returns></returns>
        //[HttpGet("update/role/{userId}/{roleId}")]
        //[IdempotentFilter]
        //public async Task<int> UpdateUserRole(int roleId, int userId)
        //{
        //    return await new BLL_SysUser(_userService, _menuService, _mapper, _logService, User, _apiUrls, Ip).UpdateUserRole(roleId, userId);
        //}
    }
}