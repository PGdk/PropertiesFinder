using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DatabaseConnection;

namespace IntegrationApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InfoController : ControllerBase
    {
        [HttpGet]
        [Route("info")]
        public IActionResult GetInfo()
        {
            return new OkObjectResult(new InfoStatus()
            {
                ConnectionString = @"Server=.\SQLEXPRESS;Database=projekt;Trusted_Connection=True;",
                IntegrationName = "otodom",
                StudentName = "Robert",
                StudentIndex = 170629
            });
        }
    }
}