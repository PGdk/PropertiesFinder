using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IntegrationApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InfoController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new
            {
                connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=mdejewski;Integrated Security=True",
                integrationName = "Noster",
                studentName = "Mateusz",
                studentIndex = 169657
            });
        }
    }
}