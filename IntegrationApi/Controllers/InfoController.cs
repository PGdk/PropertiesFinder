using DatabaseConnection;
using IntegrationApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace IntegrationApi.Controllers
{
    [ApiController]
    public class InfoController : ControllerBase
    {
        public InfoController()
        {
        }

        [HttpGet("info")]
        public IActionResult GetInfo()
        {
            return Ok(new Info()
            {
                ConnectionString = DatabaseContext.ConnectionString,
                StudentIndex = 159924,
                StudentName = "Marta",
                IntegrationName = "NieruchomosciOnline"
            });
        }
    }
}
