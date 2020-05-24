using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IntegrationAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class InfoController : ControllerBase
    {
        [HttpGet]
        public DatabaseConnection.Info Get()
        {
            var info = new DatabaseConnection.Info()
            {
                ConnectionString = @"Server=.\SQLEXPRESS;Database=stanislawk171978;Trusted_Connection=True",
                IntegrationName = "Bazos",
		        StudentName = "Stanislaw",
		        StudentIndex = 171978
            };
            return info;
        }
    }
}