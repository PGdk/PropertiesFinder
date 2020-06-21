using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatabaseConnection;
using Extensions.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;

namespace IntegrationApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize(Roles = "User,Admin")]
    public class EntriesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EntriesController(ApplicationDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<Entry>>> GetEntities(int? pageId, int? pageLimit)
        {
            IQueryable<Entry> entries = _context.Entries;

            if (pageId.HasValue != pageLimit.HasValue)
            {
                return BadRequest("Both parameters (pageId and pageLimit) must be provided.");
            }

            if(pageId.HasValue)
            {
                entries = entries
                    .OrderBy(entry => entry.Id)
                    .Skip(pageId.Value * pageLimit.Value)
                    .Take(pageLimit.Value);
            }

            return await entries.ToListAsync();
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<Entry>> GetEntry(long id)
        {
            var entry = await _context.Entries.FindAsync(id);

            if (entry == null)
            {
                return NotFound();
            }

            return entry;
        }

        // https://localhost:348/entries/Best/ANNOPOL
        [HttpGet("Best/{city}")]
        public async Task<ActionResult<ICollection<Entry>>> GetBestEntry(PolishCity city)
        {
            var entry = await _context
                .Entries
                .Where(entry1 => entry1.PropertyAddress.City == city && entry1.PropertyPrice.PricePerMeter > 0)
                .OrderByDescending(entry1 => entry1.PropertyFeatures.HasBalcony)
                .ThenByDescending(entry1 => entry1.PropertyFeatures.HasElevator)
                .ThenByDescending(entry1 => entry1.PropertyFeatures.HasBasementArea)
                .ThenByDescending(entry1 => entry1.PropertyFeatures.ParkingPlaces)
                .ThenBy(entry1 => entry1.PropertyPrice.PricePerMeter)
                .Take(5)
                .ToListAsync();

            if (entry == null)
            {
                return NotFound();
            }

            return entry;
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutEntry(long id, Entry entry)
        {
            if (id != entry.Id)
            {
                return BadRequest();
            }
            
            var dbEntry = _context.Entries.Find(id);
            dbEntry.ApplyNewValues(entry);

            _context.Entries.Update(dbEntry);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EntryExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }


        [HttpPost]
        public async Task<ActionResult<Entry>> PostEntry(Entry entry)
        {
            _context.Entries.Add(entry);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEntry", new {id = entry.Id}, entry);
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult<Entry>> DeleteEntry(int id)
        {
            var entry = await _context.Entries.FindAsync(id);
            if (entry == null)
            {
                return NotFound();
            }

            _context.Entries.Remove(entry);
            await _context.SaveChangesAsync();

            return entry;
        }

        private bool EntryExists(long id)
        {
            return _context.Entries.Any(e => e.Id == id);
        }
    }
}