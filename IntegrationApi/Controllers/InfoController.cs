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
