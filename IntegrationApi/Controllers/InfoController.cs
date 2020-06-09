using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatabaseConnection;
using IntegrationApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace IntegrationApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class InfoController : ControllerBase
    {
        [HttpGet]
        public Info GetInfo()
        {
            return new Info
            {
                ConnectionString = DatabaseContext.connectionString,
                IntegrationName = "Lento",
                StudentName = "Jan Wesołek",
                StudentIndex = 155223
            };
        }
    }
}