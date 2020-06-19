using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DatabaseConnection;
using Models;

namespace IntegrationApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class EntriesController : ControllerBase
    {
        private readonly EntryContext _context;

        public EntriesController(EntryContext context)
        {
            _context = context;
        }

        // GET: Entries
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Entry>>> GetEntries()
        {
            return await _context.Entries
                .Include(e => e.OfferDetails)
                .ThenInclude(od => od.SellerContact)
                .Include(e => e.PropertyAddress)
                .Include(e => e.PropertyDetails)
                .Include(e => e.PropertyPrice)
                .Include(e => e.PropertyFeatures).ToListAsync();
        }

        // GET: Entries/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Entry>> GetEntry(int id)
        {
            var entry = await _context.Entries
                .Include(e => e.OfferDetails)
                .ThenInclude(od => od.SellerContact)
                .Include(e => e.PropertyAddress)
                .Include(e => e.PropertyDetails)
                .Include(e => e.PropertyPrice)
                .Include(e => e.PropertyFeatures).FirstAsync(e => e.Id.Equals(id));
            if (entry == null)
            {
                return NotFound();
            }

            return entry;
        }

        // PUT: Entries/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEntry(int id, Entry entry)
        {
            if (id != entry.Id)
            {
                return BadRequest();
            }

            _context.Entry(entry).State = EntityState.Modified;

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

                throw;
            }

            return NoContent();
        }

        // POST: Entries
        [HttpPost]
        public async Task<ActionResult<Entry>> PostEntry(Entry entry)
        {
            _context.Entries.Add(entry);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEntry", new { id = entry.Id }, entry);
        }

        // DELETE: Entries/5
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

        private bool EntryExists(int id)
        {
            return _context.Entries.Any(e => e.Id == id);
        }
    }
}
