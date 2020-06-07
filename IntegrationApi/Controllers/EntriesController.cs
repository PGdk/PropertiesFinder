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
        public IActionResult Get(int pageLimit = -1, int pageId = -1)
        {
            if (pageLimit == -1 || pageId == -1)
                return Ok(DatabaseManager.GetAllEntries());

            int startingEntry = pageLimit * (pageId - 1);
            var entries = DatabaseManager.GetEntriesForGivenRange(startingEntry, pageLimit);

            return Ok(entries);
        }
    }
}