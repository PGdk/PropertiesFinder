using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntegrationApi.Polices
{
    public class AdminPolicy : AuthorizationHandler<AdminPolicyRequirement>
    {
        const string GoogleEmailAddressSchema = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress";
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AdminPolicyRequirement requirement)
        {
            var email = context.User.Claims.FirstOrDefault(p => p.Issuer.Equals("Google") && p.Type.Equals(GoogleEmailAddressSchema));

            if(email != null)
            {
                if (email.Value.Equals("piotr.tybura@gmail.com") || email.Value.Equals("lab03dummy@gmail.com"))
                {
                    context.Succeed(requirement);
                }
            }
            return Task.CompletedTask;
        }
    }
}
