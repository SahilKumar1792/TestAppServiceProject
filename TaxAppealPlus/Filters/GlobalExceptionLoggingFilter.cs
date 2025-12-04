using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;

namespace TaxAppealPlus.Filters
{
    public class GlobalExceptionLoggingFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            var controller = context.RouteData.Values["controller"]?.ToString() ?? "UnknownController";
            var action = context.RouteData.Values["action"]?.ToString() ?? "UnknownAction";

            Log.ForContext("Controller", controller)
               .ForContext("Action", action)
               .ForContext("Path", context.HttpContext.Request.Path)
               .ForContext("QueryString", context.HttpContext.Request.QueryString.ToString())
               .Error(context.Exception, "Unhandled exception in {Controller}/{Action}");
        }
    }
}


