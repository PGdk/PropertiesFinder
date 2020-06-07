using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace IntegrationApi.Controllers
{
    [Route("/")]
    [ApiController]
    public class StatusController : ControllerBase
    {
        private readonly IConfiguration conf;

        public StatusController(IConfiguration configuration)
        {
            conf = configuration;
        }

        [Route("info")]
        [HttpGet]
        public IActionResult Info()
        {
            var info = new { connectionString = conf.GetConnectionString("PortaleDbContext"), integrationName = "Portale Nieruchomosci", studentName = "Monika", studentIndex = "142691" };
            return Content(info.ToString());
        }
    }
}