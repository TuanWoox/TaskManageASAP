using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using WebApplication1.DTOs.TaskDTO;

namespace WebApplication1.Filters
{
    public class IsAdmin: ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var userClaims = context.HttpContext.User.Identity as ClaimsIdentity;
            var role = userClaims?.FindFirst(ClaimTypes.Role)?.Value;

            if(role != "Admin")
            {
                // Block non-admin users
                context.Result = new ForbidResult();
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}
