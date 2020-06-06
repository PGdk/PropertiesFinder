using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatabaseConnection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;

namespace IntegrationApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class EntriesController : ControllerBase
    {
        [HttpGet("{id}")]
        [Authorize]
        [Authorize(Policy = "User")]
        public async Task<ActionResult<Entry>> GetEntry(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            using DatabaseContext databaseContext = new DatabaseContext();
            Entry entry = await databaseContext.Entries.Include(e => e.OfferDetails).ThenInclude(e => e.SellerContact)
                            .Include(e => e.PropertyAddress)
                            .Include(e => e.PropertyDetails)
                            .Include(e => e.PropertyFeatures)
                            .Include(e => e.PropertyPrice).Where(e => e.Id == id).SingleOrDefaultAsync();

            if(entry != null)
            {
                return Ok(entry);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet("{pageLimit}/{pageId}")]
        [Authorize]
        [Authorize(Policy = "User")]
        public async Task<ActionResult<List<Entry>>> GetEntries(int pageLimit, int pageId)
        {
            if(pageLimit <= 0 || pageId <= 0)
            {
                return BadRequest();
            }

            int fromEntryId = pageLimit * (pageId - 1) + 1;
            int toEntryId = fromEntryId + pageLimit;

            using DatabaseContext databaseContext = new DatabaseContext();
            List<Entry> entries = await databaseContext.Entries.Include(e => e.OfferDetails).ThenInclude(e => e.SellerContact)
                            .Include(e => e.PropertyAddress)
                            .Include(e => e.PropertyDetails)
                            .Include(e => e.PropertyFeatures)
                            .Include(e => e.PropertyPrice)
                            .Where(entry => entry.Id >= fromEntryId)
                            .Where(entry => entry.Id < toEntryId)
                            .ToListAsync();

            if (entries.Count > 0)
            {
                return Ok(entries);
            }
            else
            {
                return NotFound(); 
            }

        }

        [HttpGet]
        [Authorize]
        [Authorize(Policy = "User")]
        public async Task<ActionResult<List<Entry>>> GetEntries()
        {
            using DatabaseContext databaseContext = new DatabaseContext();
            List<Entry> entries = await databaseContext.Entries.Include(e => e.OfferDetails).ThenInclude(e => e.SellerContact)
                            .Include(e => e.PropertyAddress)
                            .Include(e => e.PropertyDetails)
                            .Include(e => e.PropertyFeatures)
                            .Include(e => e.PropertyPrice)
                            .ToListAsync();

            if (entries.Count > 0)
            {
                return Ok(entries);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        [Authorize(Policy = "Admin")]
        public async Task<ActionResult<Entry>> UpdateEntry(int id, Entry entry)
        {
            if (id <= 0 || id != entry.Id)
            {
                return BadRequest();
            }

            using DatabaseContext databaseContext = new DatabaseContext();
            databaseContext.Update(entry);
            await databaseContext.SaveChangesAsync();

            return Ok(entry);
        }
    }
}
