using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;

namespace Sikiro.SMS.Api.Handler
{
    /// <summary>
    /// swagger添加token header
    /// </summary>
    public class AuthTokenHeaderFilter : IOperationFilter
    {
        /// <summary>
        /// swagger添加请求头
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="context"></param>
        public void Apply(Operation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
                operation.Parameters = new List<IParameter>();

            foreach (IParameter param in operation.Parameters)
            {
                if (param.In == "modelbinding")
                    param.In = "query";
            }

            operation.Parameters.Add(new NonBodyParameter()
            {
                Name = "Authorization",
                In = "header",
                Type = "string",
                Required = false,
                Default = "Bearer ",
                Description = "Bearer Token"
            });
        }
    }
}
