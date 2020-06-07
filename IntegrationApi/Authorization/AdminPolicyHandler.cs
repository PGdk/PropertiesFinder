using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IntegrationApi.Authorization
{
    public class AdminPolicyHandler : AuthorizationHandler<AdminPolicyRequirement>
    {
        private List<string> AdminEmails { get; set; } = new List<string>() { "piotr.tybura@gmail.com", "nikgracz@student.pg.gda.pl" };
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AdminPolicyRequirement requirement)
        {
            var email = context.User.Claims
                .Where(c => c.Type == ClaimTypes.Email)
                .FirstOrDefault()?.Value;

            if (AdminEmails.Contains(email))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }

    }
}
