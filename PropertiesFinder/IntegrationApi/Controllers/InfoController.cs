using DatabaseConnection;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace IntegrationAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class InfoController : ControllerBase
    {

        // GET: info
        [HttpGet]
        public DatabaseConnection.Info Get()
        {
            var info = new DatabaseConnection.Info()
            {
                ConnectionString = @"Data Source=.\\SQLEXPRESS;Initial Catalog=DariuszK170624;Integrated Security=True",
                IntegrationName = "Domiporta",
                StudentName = "Dariusz",
                StudentIndex = "170624"
            };
            return info;
        }
    }
}
