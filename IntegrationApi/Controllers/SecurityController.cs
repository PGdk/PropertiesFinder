using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IntegrationApi.Controllers
{
    [ApiController]
    public class SecurityController : ControllerBase
    {
        [Route("/security/access_denied")]
        [Authorize(Policy = "User")]
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return Unauthorized("Access denied!");
        }
    }
}
