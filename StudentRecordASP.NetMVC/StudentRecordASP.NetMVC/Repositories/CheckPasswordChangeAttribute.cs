using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace StudentRecordASP.NetMVC.Repositories
{
    public class CheckPasswordChangeAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Check if user is authenticated
            if (context.HttpContext.User.Identity.IsAuthenticated)
            {
                // Get the claim
                var isDefaultPasswordClaim = context.HttpContext.User.FindFirst("IsDefaultPassword");

                if (isDefaultPasswordClaim != null && bool.Parse(isDefaultPasswordClaim.Value))
                {
                    // Get the current controller and action
                    string controller = context.RouteData.Values["controller"]?.ToString() ?? "";
                    string action = context.RouteData.Values["action"]?.ToString() ?? "";

                    // Allow access ONLY to the ChangePassword page or Logout
                    if (controller != "Account" || (action != "ChangePassword" && action != "Logout"))
                    {
                        // Redirect all other requests
                        context.Result = new RedirectToActionResult("ChangePassword", "Account", null);
                    }
                }
            }
            base.OnActionExecuting(context);
        }
    }
}
