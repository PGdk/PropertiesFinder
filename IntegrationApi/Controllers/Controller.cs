using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatabaseConnection;
using DatabaseConnection.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace IntegrationApi.Controllers
{
    [Route("/")]
    [ApiController]
    public class Controller : ControllerBase
    {

        private readonly IConfiguration _configuration;
        private readonly DatabaseAccessService _databaseAccessService;

        public Controller(IConfiguration configuration, DatabaseAccessService databaseAccessService)
        {
            _configuration = configuration;
            _databaseAccessService = databaseAccessService;
        }

        [Route("info")]
        [Authorize(Policy = "Admin")]
        [HttpGet]
        public IActionResult GetInfo()
        {
            try
            {
                return Ok(new ConfigInfo()
                {
                    connectionString = _configuration["ConnectionStrings:DatabaseConnection"],
                    integrationName = _configuration["IntegrationName"],
                    studentIndex = _configuration["ConnectionStrings:DatabaseConnection"],
                    studentName = _configuration["ConnectionStrings:DatabaseConnection"]
                });
            }catch(Exception ex)
            {
                //log the exception
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
            
        }


        [Route("page")]
        [Authorize(Policy = "User")]
        [HttpPost]
        public IActionResult LoadPage()
        {

            return NoContent();
        }
        
        [Route("entry/{id}")]
        [Authorize(Policy = "User")]
        [HttpGet]
        public IActionResult GetEntry(int id)
        {
            Entry entry = _databaseAccessService.getEntry(id);
            if (entry == null)
            {
                return NotFound();
            }
            return Ok(entry);
        }

        [Route("entry")]
        [Authorize(Policy = "User")]
        [HttpGet]
        public IActionResult GetEntries()
        {
            List<Entry> entries = _databaseAccessService.getEntries();
            if (entries.Count == 0)
            {
                return NotFound();
            }
            return Ok(entries);
        }

        [Route("entry")]
        [Authorize(Policy = "Admin")]
        [HttpPut]
        public IActionResult PutEntry(Entry entry)
        {
            int n = _databaseAccessService.putEntry(entry);
            if (n == 0)
            {
                return BadRequest();
            }
            return Ok(entry);
        }
    }
}
