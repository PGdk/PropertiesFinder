using DatabaseConnection;
using DatabaseConnection.Models;
using Microsoft.AspNetCore.Mvc;

namespace IntegrationApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class InfoController : ControllerBase
    {
        private readonly IGazetaKrakowskaRepository databaseRepository;
        public InfoController(IGazetaKrakowskaRepository databaseRepository)
        {
            this.databaseRepository = databaseRepository;
        }
        [HttpGet]
        public IActionResult GetInfo()
        {
            if (Request.Headers.ContainsKey("X-Request-ID"))
            {
                if (Request.Headers["X-Request-ID"].ToString().Trim() == "")
                    return new BadRequestObjectResult("X-Request-ID cannot be empty");

                databaseRepository.AddLog(Request.Headers["X-Request-ID"]);
            }

            return new OkObjectResult(new InfoStatus()
            {
                ConnectionString = @"Server=.\SQLEXPRESS;Database=KamilD170100;Trusted_Connection=True;",
                IntegrationName = "gazeta krakowska",
                StudentName = "Kamil",
                StudentIndex = 170100
            });
        }
    }
}
