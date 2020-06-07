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
    public class EntryController : ControllerBase
    {
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            Entry entry = DatabaseManager.GetAllEntries().FirstOrDefault(e => e.Id == id);

            if (entry == null)
                return BadRequest("Error - podana oferta nie istnieje");
            return Ok(entry);
        }
    }
}