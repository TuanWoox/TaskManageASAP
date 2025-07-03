using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using WebApplication1.DTOs.TaskDTO;

public class RequireAdminForDoneStatus : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var userClaims = context.HttpContext.User.Identity as ClaimsIdentity;
        var role = userClaims?.FindFirst(ClaimTypes.Role)?.Value;

        if (context.ActionArguments.TryGetValue("updateTaskDTO", out var obj))
        {
            var model = obj as UpdateTaskDTO;
            if (model != null)
            {
                if (model.Status?.ToLower() == "done")
                {
                    if (role != "Admin")
                    {
                        // Block non-admin users
                        context.Result = new ForbidResult();
                        return;
                    }
                }
            }
        }

        base.OnActionExecuting(context);
    }
}
