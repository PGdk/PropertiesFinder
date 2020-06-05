using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatabaseConnection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IntegrationApi.Controllers
{
    [Route("")]
    [ApiController]
    public class InfoController : ControllerBase
    {
        private readonly ISprzedajemyRepository databaseRepository;
        public InfoController(ISprzedajemyRepository databaseRepository)
        {
            this.databaseRepository = databaseRepository;
        }
        [HttpGet]
        [Route("info")]
        //[Authorize(Policy = "Admin")]
        public IActionResult GetInfo()
        {
            return new OkObjectResult(new InfoStatus()
            {
                ConnectionString = @"Server=.\SQLEXPRESS;Database=WojciechG171706;Trusted_Connection=True;",
                IntegrationName = "sprzedajemy",
                StudentName = "Wojciech",
                StudentIndex = 171706
            });
        }
    }
}