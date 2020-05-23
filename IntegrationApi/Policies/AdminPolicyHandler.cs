using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace IntegrationApi
{
    public class AdminPolicyHandler : AuthorizationHandler<AdminPolicyRequirement>
    {
        const string GoogleEmailAddressSchema = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress";
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AdminPolicyRequirement requirement)
        {
            var email = context.User.Claims.FirstOrDefault(p => p.Issuer.Equals("Google") && p.Type.Equals(GoogleEmailAddressSchema));

            if (email != null && email.Value.Equals("piotr.tybura@gmail.com"))
                context.Succeed(requirement);
            if (email != null && email.Value.Equals("kamil.demkowicz95@gmail.com"))
                context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}
