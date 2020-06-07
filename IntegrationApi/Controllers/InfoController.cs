using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatabaseConnection;
using IntegrationApi.Models;
using Microsoft.AspNetCore.Http;
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
                ConnectionString = DatabaseContext.CONNECTION_STRING,
                IntegrationName = "EUchodnia",
                StudentName = "Nikodem Graczewski",
                StudentIndex = 143227
            };
        }
    }
}