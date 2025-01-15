using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CREDA_App.Helper
{
    public class Authentication : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Session.GetString("User") == null)
            {
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary {
                        { "Controller", "Home" },
                        { "Action", "Index" }
                    });
            }
        }
    }
}