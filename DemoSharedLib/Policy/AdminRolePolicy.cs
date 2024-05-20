namespace DemoSharedLib.Policy;

using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

public class AdminRolePolicy : AuthorizationHandler<AdminRolePolicy>, IAuthorizationRequirement
{
    public const string PolicyName = "AdminPolicy";

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AdminRolePolicy rolePolicy)
    {
        var user = context.User.Identities.First();

        var first = user.Claims.First(x => x.Type == ClaimTypes.Email);

        if (first.Value == "admin")
        {
            context.Succeed(rolePolicy);

            return Task.CompletedTask;
        }

        context.Fail();

        return Task.CompletedTask;
    }
}
