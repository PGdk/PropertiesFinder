using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace IntegrationApi.Security
{
    public class AdminPolicyHandler : AuthorizationHandler<AdminPolicyRequirement>
    {
        private static readonly string GoogleIssuer = "Google";
        private static readonly string GoogleEmailAddressSchema = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress";
        private static readonly string[] Admins = { "kamilwerner50@gmail.com", "piotr.tybura@gmail.com" };

        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            AdminPolicyRequirement requirement
        ) {
            var email = context.User.Claims.FirstOrDefault(
                p => p.Issuer.Equals(GoogleIssuer) && p.Type.Equals(GoogleEmailAddressSchema)
            );

            if (null != email && Array.Exists(Admins, a => email.Value == a))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
