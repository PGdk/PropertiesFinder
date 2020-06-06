using DatabaseConnection;
using Microsoft.AspNetCore.Mvc;

namespace IntegrationApi.Controllers
{
    [Route("")]
    [ApiController]
    public class InfoController : ControllerBase
    {
        [HttpGet]
        [Route("info")]
        public IActionResult GetInfo()
        {
            return new OkObjectResult(new InfoStatus()
            {
                ConnectionString = @"Server=.\SQLEXPRESS;Database=MaciejK169013;Trusted_Connection=True;",
                IntegrationName = "trovit",
                StudentName = "Maciej",
                StudentIndex = 169013
            });
        }
    }
}