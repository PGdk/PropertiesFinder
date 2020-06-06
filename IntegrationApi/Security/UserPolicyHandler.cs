using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace IntegrationApi.Security
{
    public class UserPolicyHandler : AuthorizationHandler<UserPolicyRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            UserPolicyRequirement requirement
        ) {
            if (context.User.Identity.IsAuthenticated)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
