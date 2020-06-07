using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntegrationApi.Authorization
{
    public class UserPolicyHandler : AuthorizationHandler<UserPolicyRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, UserPolicyRequirement requirement)
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}
