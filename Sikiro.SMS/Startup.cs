using EasyNetQ.Scheduling;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using Sikiro.Nosql.Mongo;
using Sikiro.SMS.Api.Helper;
using Swashbuckle.AspNetCore.Swagger;
using System.IO;
using System.Reflection;

namespace Sikiro.SMS.Api
{
    public class Startup
    {
        private readonly InfrastructureConfig _infrastructureConfig;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            _infrastructureConfig = configuration.Get<InfrastructureConfig>();
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            #region Swagger配置
            services.ConfigureSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Info { Title = "SMS API", Version = "v1", Description = "RESTful API for My Web Application", TermsOfService = "None" });
                //注意：此处替换成所生成的XML documentation的文件名[ bin\Debug\netcoreapp2.1\Kjs.Pay.Api.xml]                
                options.IncludeXmlComments(Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, string.Format("{0}.xml", MethodBase.GetCurrentMethod().DeclaringType.Namespace)));
                options.DescribeAllEnumsAsStrings();

            });
            services.AddSwaggerGen();

            #endregion


            services.AddMvc(option =>
                {
                    option.Filters.Add<GolbalExceptionAttribute>();
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddHttpContextAccessor();
            services.RegisterEasyNetQ(_infrastructureConfig.Infrastructure.RabbitMQ, a =>
            {
                a.EnableDeadLetterExchangeAndMessageTtlScheduler();
            });
            services.AddSingleton(new MongoRepository(_infrastructureConfig.Infrastructure.Mongodb));
            services.AddService();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {


            if (env.IsEnvironment("develop") || env.IsEnvironment("test"))
            {
                app.UseDeveloperExceptionPage();

                app.UseSwagger();
                app.UseSwaggerUI(c => {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "KJS Pay API V1");
                });
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

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