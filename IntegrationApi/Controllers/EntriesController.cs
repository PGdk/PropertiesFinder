using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatabaseConnection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;

namespace IntegrationApi.Controllers
{
    [Route("[controller]")]
    [Authorize]
    [ApiController]
    public class EntriesController : ControllerBase
    {
        private DatabaseContext DatabaseContext { get; set; }

        public EntriesController(DatabaseContext databaseContext)
        {
            DatabaseContext = databaseContext;
        }

        [HttpGet]
        [Authorize(Policy = "User")]
        public IActionResult GetEntries([FromQuery(Name = "PageId")] int? pageId, [FromQuery(Name = "PageLimit")] int? pageLimit)
        {
            var query = DatabaseContext.Entries
                .Include(e => e.OfferDetails)
                    .ThenInclude(od => od.SellerContact)
                .Include(e => e.PropertyAddress)
                .Include(e => e.PropertyDetails)
                .Include(e => e.PropertyPrice)
                .Include(e => e.PropertyFeatures);

            if (pageId == null && pageLimit == null)
            {
                return Ok(query.ToList());
            }

            if (pageId != null && pageLimit != null)
            {
                return Ok(query
                    .Skip(((int)pageId - 1) * (int)pageLimit)
                    .Take((int)pageLimit)
                    .ToList());
            }

            return BadRequest("Both PageId and PageLimit must be given");
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "Admin")]
        public IActionResult GetEntry(int id)
        {
            var entry = DatabaseContext.Entries
                .Include(e => e.OfferDetails)
                    .ThenInclude(od => od.SellerContact)
                .Include(e => e.PropertyAddress)
                .Include(e => e.PropertyDetails)
                .Include(e => e.PropertyPrice)
                .Include(e => e.PropertyFeatures)
                .Where(e => e.ID == id)
                .FirstOrDefault();

            if (entry == null)
            {
                return NotFound($"No entry with the given id: {id}");
            }

            return Ok(entry);
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "Admin")]
        public IActionResult PutEntry(int id, [FromBody] Entry entry)
        {
            DatabaseContext.Entries.Update(entry);
            DatabaseContext.SaveChanges();

            return Ok();
        }
    }
}