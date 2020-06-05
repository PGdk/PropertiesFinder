using DatabaseConnection;
using Exhouse.Exhouse;
using IntegrationApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace IntegrationApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class InfoController : ControllerBase
    {
        private static readonly string StudentName = "Kamil Werner";
        private static readonly int StudentIndex = 167950;

        // GET: /info
        [HttpGet]
        public ActionResult<Info> Get()
        {
            return Ok(
                new Info
                {
                    ConnectionString = DatabaseContext.ConnectionString,
                    IntegrationName = ExhouseWebPage.NAME,
                    StudentName = StudentName,
                    StudentIndex = StudentIndex
                }
            );
        }
    }
}