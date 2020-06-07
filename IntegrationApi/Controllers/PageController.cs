using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatabaseConnection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using Application;

namespace IntegrationApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PageController : ControllerBase
    {
        [HttpPost("{id}")]
        public IActionResult Post(int id)
        {
            List<Entry> entries = new List<Entry>();
            entries = NosterIntegration.GetEntriesForPage(id);
            if (entries == null)
                return BadRequest("Error - podana strona nie istnieje");

            DatabaseManager.AddEntriesToDatabase(entries);
            return NoContent();
        }
    }
}