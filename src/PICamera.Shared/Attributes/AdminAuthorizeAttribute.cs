using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using PICamera.Shared.Extensions;

namespace PICamera.Shared.Attributes
{
    public class AdminAuthorizeAttribute : Attribute, IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {

            if (context.HttpContext.Request.Headers.TryGetValue("admin-access-key", out StringValues token))
            {
                IConfiguration configuration = context.HttpContext.RequestServices.GetService<IConfiguration>();

                if (Guid.TryParse(token, out Guid adminKey) && configuration.TryGetValue("AdminAccessKey", out string key))
                {
                    if (adminKey != Guid.Parse(key))
                        context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
                }
                else
                {
                    context.Result = new JsonResult(new { message = "Missing token." }) { StatusCode = StatusCodes.Status400BadRequest };
                }
            }
            else
            {
                context.Result = new JsonResult(new { message = "Missing token." }) { StatusCode = StatusCodes.Status400BadRequest };
            }
        }
    }
}
