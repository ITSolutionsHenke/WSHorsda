using Microsoft.AspNetCore.Mvc.Filters;

/// <summary>
/// https://stackoverflow.com/questions/6290053/setting-access-control-allow-origin-in-asp-net-mvc-simplest-possible-method
/// </summary>

public class AllowCrossSiteJsonAttribute : ActionFilterAttribute
{
   public override void OnActionExecuting(ActionExecutingContext filterContext)
   {
      filterContext.HttpContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");
      base.OnActionExecuting(filterContext);
   }
}