using EasyNetQ.Scheduling;
using Kj.Com.JWT;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.IdentityModel.Tokens;
using NLog.Extensions.Logging;
using NLog.Web;
using Sikiro.Nosql.Mongo;
using Sikiro.SMS.Api.Handler;
using Sikiro.SMS.Api.Helper;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Sikiro.SMS.Api
{
    /// <summary>
    /// 
    /// </summary>
    public class Startup
    {
        private readonly InfrastructureConfig _infrastructureConfig;
        public IConfiguration Configuration { get; }

        /// <summary>
        /// 配置根节点类接口
        /// </summary>
        public IConfigurationRoot ConfigurationRoot { get; set; }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="env"></param>
        public Startup(IHostingEnvironment env, IConfiguration configuration)
        {
            Configuration = configuration;
            _infrastructureConfig = configuration.Get<InfrastructureConfig>();

            var builder = new ConfigurationBuilder()  //创建ConfigurationBuilder，其作用就是加载Congfig等配置文件 
            .SetBasePath(env.ContentRootPath)  //env.ContentRootPath：获取当前项目的跟路径 
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)  //使用AddJsonFile方法把项目中的appsettings.json配置文件加载进来，后面的reloadOnChange顾名思义就是文件如果改动就重新加载 
            .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true) //关注的是$"{param}"的这种写法，有点类似于string.Format() 
            .AddEnvironmentVariables();

            ConfigurationRoot = builder.Build(); //返回一个配置文件跟节点：IConfigurationRoot 
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            #region jwt认证配置

            //添加跨域支持
            services.AddCors(options =>
            options.AddPolicy("Any", builder => builder.AllowAnyMethod().AllowAnyHeader().AllowAnyOrigin().AllowCredentials()));

            //读取配置文件
            var audienceConfig = ConfigurationRoot.GetSection("Audience");
            var symmetricKeyAsBase64 = audienceConfig["Secret"];
            var keyByteArray = Encoding.ASCII.GetBytes(symmetricKeyAsBase64);
            var signingKey = new SymmetricSecurityKey(keyByteArray);
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,
                ValidateIssuer = true,
                ValidIssuer = audienceConfig["Issuer"],//发行人
                ValidateAudience = true,
                ValidAudience = audienceConfig["Audience"],//订阅人
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                RequireExpirationTime = true,

            };
            var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
            //string[] SystemModuleArry = { "log", "menu", "role", "user"}, PlatformModuleArry = { "advertCarousel", "adverts", "articlecategory", "article", "articlegrouprule", "articlespec", "articlesupplier", "oauth", "orders", "payment" };

            //var permissions = new List<JwtPermission> {
            //    new JwtPermission(){ RoleName="System",ModuleArryList=SystemModuleArry }, //系统管理员
            //    new JwtPermission(){ RoleName="Operator",ModuleArryList=PlatformModuleArry}, //系统操作员
            //};
            var jwtRequirement = new JwtRequirement(
                //permissions,
                "/api/user/denied",
                ClaimTypes.Role,
                audienceConfig["Issuer"],
                audienceConfig["Audience"],
                signingCredentials,
                expiration: TimeSpan.FromMinutes(30)//设置Token过期时间
                );


            services.AddAuthorization(options =>
            {
                options.AddPolicy("Bearer", policy => policy.Requirements.Add(jwtRequirement));

            })
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(o =>
            {
                o.RequireHttpsMetadata = false;
                o.TokenValidationParameters = tokenValidationParameters;
                o.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        if (context.Request.Path.Value.ToString() == "/api/user/logout")
                        {
                            var token = ((context as TokenValidatedContext).SecurityToken as JwtSecurityToken).RawData;
                        }
                        return Task.CompletedTask;
                    }
                };
            });
            //注入授权Handler
            services.AddSingleton<IAuthorizationHandler, JwtAuthorizationHandler>();
            services.AddSingleton(jwtRequirement);

            #endregion


            #region Swagger配置
            services.ConfigureSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Info { Title = "SMS API", Version = "v1", Description = "RESTful API for My Web Application", TermsOfService = "None" });
                //注意：此处替换成所生成的XML documentation的文件名[ bin\Debug\netcoreapp2.1\Kjs.Pay.Api.xml]                
                options.IncludeXmlComments(Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, string.Format("{0}.xml", MethodBase.GetCurrentMethod().DeclaringType.Namespace)));
                options.DescribeAllEnumsAsStrings();
                //手动高亮
                options.OperationFilter<AuthTokenHeaderFilter>();

            });
            services.AddSwaggerGen();

            #endregion

            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(ApiResultHandlerMiddleware));
                options.RespectBrowserAcceptHeader = true;
            })
            .AddJsonOptions(options =>
            {
                options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
            });

            //services.AddHttpContextAccessor();
            services.RegisterEasyNetQ(_infrastructureConfig.Infrastructure.RabbitMQ, a =>
            {
                a.EnableDeadLetterExchangeAndMessageTtlScheduler();
            });
            services.AddSingleton(new MongoRepository(_infrastructureConfig.Infrastructure.Mongodb));
            services.AddService();
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <param name="loggerFactory"></param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            // 添加日志支持
            //loggerFactory.AddConsole(ConfigurationRoot.GetSection("Logging"));
            loggerFactory.AddDebug();

            //添加NLog
            loggerFactory.AddNLog();
            //读取Nlog配置文件 
            env.ConfigureNLog("nlog.config");

            app.UseCors("Any");

            if (env.IsEnvironment("develop") || env.IsEnvironment("test"))
            {
                app.UseDeveloperExceptionPage();

                app.UseSwagger();
                app.UseSwaggerUI(c => {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "KJS Pay API V1");
                });
            }

            //错误
            app.UseErrorHandling();

            app.UseMvc();
        }
    }

    internal class InfrastructureConfig
    {
        public Infrastructure Infrastructure { get; set; }
    }

    public class Infrastructure
    {
        public string Mongodb { get; set; }
        public string RabbitMQ { get; set; }
    }
}