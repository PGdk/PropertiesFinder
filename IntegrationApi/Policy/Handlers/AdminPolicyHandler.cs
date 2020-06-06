using IntegrationApi.Policy.Models;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntegrationApi.Policy.Handlers
{
    public class AdminPolicyHandler : AuthorizationHandler<AdminPolicyRequirement>
    {
        const string GoogleEmailAddressSchema =
        "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress";
        protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        AdminPolicyRequirement requirement)
        {
            var email = context.User.Claims.FirstOrDefault(p =>p.Issuer.Equals("Google") && p.Type.Equals(GoogleEmailAddressSchema));

            if (email != null && (email.Value.Equals("piotr.tybura@gmail.com") || email.Value.Equals("student.jan.miotk@gmail.com")))
                context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}
