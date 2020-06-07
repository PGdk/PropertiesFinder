using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatabaseConnection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;

namespace IntegrationApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EntriesController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            List<Entry> entries = DatabaseManager.GetAllEntries();
            if (entries == null)
                return BadRequest("Error - nie dodano jeszcze żadnych ofert");
            return Ok(entries);
        }

        [HttpGet("{pageLimit}/{pageId}")]
        public IActionResult Get(int pageLimit, int pageId)
        {
            var allEntries = DatabaseManager.GetAllEntries();
            if (allEntries == null)
                return BadRequest("Error - nie dodano jeszcze żadnych ofert");

            int startingEntry = pageId * pageLimit + 1;
            int lastEntry = pageId * pageLimit + pageLimit;

            var entries = new List<Entry>();
            for (int i = startingEntry; i <= lastEntry; i++)
            {
                entries.Add(allEntries[i - 1]);
            }

            return Ok(entries);

        }

    }
}