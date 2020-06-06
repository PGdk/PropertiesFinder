using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DatabaseConnection.Models;
using DatabaseConnection;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace IntegrationApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class InfoController : ControllerBase
    {
        private readonly DatabaseContext _database;

        public InfoController(DatabaseContext database)
        {
            _database = database;
        }

        // GET: api/<InfoController>
        [HttpGet]
        public IActionResult GetInfo()
        {
            if (Request.Headers.ContainsKey("X-Request-ID"))
            {
                _database.Logs.Add(new Logs()
                {
                    date = DateTime.Now,
                    xRequestID = Request.Headers["X-Request-ID"]
                });
                _database.SaveChanges();
            }

            var info = new Info()
            {
                connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=maciejk120486;Integrated Security=True",
                integrationName = "AdresowoPL",
                studentIndex = 120486,
                studentName = "Maciej"
            };
            return new OkObjectResult (info);
        }
    }
}
