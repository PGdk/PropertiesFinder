using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DatabaseConnection;
using Models;
using Microsoft.AspNetCore.Authorization;

namespace IntegrationAPI.Controllers
{
    [Route("")]
    [Authorize]
    [ApiController]
    public class EntriesController : ControllerBase
    {
        private readonly BazosContext _context;

        public EntriesController(BazosContext context)
        {
            _context = context;
        }

        // GET: api/Entries
        [HttpGet]
        [Route("Entries")]
        public async Task<ActionResult<IEnumerable<Entry>>> GetEntries()
        {

            return await _context.Entries
            .Include(entry => entry.OfferDetails)
                .ThenInclude(offerDetails => offerDetails.SellerContact)
            .Include(entry => entry.PropertyAddress)
            .Include(entry => entry.PropertyDetails)
            .Include(entry => entry.PropertyFeatures)
            .Include(entry => entry.PropertyPrice).ToListAsync();
        }

        //DODATKOWE: PageLimit i PageId
        // GET: api/Entries
        [HttpGet]
        [Route("Entries/PageLimit/{pageLimit}/PageId/{pageId}")]
        public async Task<ActionResult<IEnumerable<Entry>>> GetEntriesFromPage(int pageLimit, int pageId)
        {
            return await _context.Entries
            .Include(entry => entry.OfferDetails)
                .ThenInclude(offerDetails => offerDetails.SellerContact)
            .Include(entry => entry.PropertyAddress)
            .Include(entry => entry.PropertyDetails)
            .Include(entry => entry.PropertyFeatures)
            .Include(entry => entry.PropertyPrice).ToListAsync();
        }

        // GET: api/Entry/5
        [HttpGet("{id}")]
        [Route("Entry/{id}")]
        public async Task<ActionResult<Entry>> GetEntry(int id)
        {
            var entryList = await _context.Entries
            .Include(entry => entry.OfferDetails)
                .ThenInclude(offerDetails => offerDetails.SellerContact)
            .Include(entry => entry.PropertyAddress)
            .Include(entry => entry.PropertyDetails)
            .Include(entry => entry.PropertyFeatures)
            .Include(entry => entry.PropertyPrice).ToListAsync();
            Entry entry = null;

            for (int i=0; i<entryList.Count; i++)
            {
                if (entryList[i].Id == id)
                    entry = entryList[i];
            }

            if (entry == null)
            {
                return NotFound();
            }
            return entry;
        }

        //DODATKOWE - aktualizowanie danych
        // PUT: api/Entry/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        [Route("PutEntry/{id}")]
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
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Page
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        [Route("Page")]
        public async Task<ActionResult<Entry>> PostEntry(Entry entry)
        {
            _context.Entries.Add(entry);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEntry", new { id = entry.Id }, entry);
        }

        // DELETE: api/Entry/5
        [HttpDelete("{id}")]
        [Route("DeleteEntry/{id}")]
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
