using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DatabaseConnection;
using Models;
using Application.DobryAdres;
using System.Text.Json;
using DobryAdres;

namespace IntegrationApi.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    public class EntriesController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly BestDealsLogic _bestDealsLogic;

        public EntriesController(DatabaseContext context)
        {
            _context = context;
            _bestDealsLogic = new BestDealsLogic();
        }


        //============================== Etap 3 ======================================
        // GET: /Entries/best/szczecin
        [HttpGet("best/{city}")]
        public ActionResult<IEnumerable<Entry>> GetBestDeals(string city)
        {
            var entries = _context.Entries.Include(x => x.PropertyPrice)
                 .Include(x => x.PropertyDetails).Include(x => x.PropertyAddress).Include(x => x.PropertyFeatures)
                 .Include(x => x.OfferDetails).ThenInclude(x => x.SellerContact).ToList();

            if (entries.Count() == 0)
            {
                return NotFound();
            }

            List<Entry> bestEntries = _bestDealsLogic.FindBestDeals(entries, city);
            if (bestEntries == null)
            {
                return NotFound();
            }

            return Ok(bestEntries);
        }
        //============================== Etap 3 ======================================


        // GET: /Entries
        [HttpGet]
        public ActionResult<IEnumerable<Entry>> GetEntries()
        {
            var entries = _context.Entries.Include(x => x.PropertyPrice)
                 .Include(x => x.PropertyDetails).Include(x => x.PropertyAddress).Include(x => x.PropertyFeatures)
                 .Include(x => x.OfferDetails).ThenInclude(x => x.SellerContact).ToList();
            if (entries.Count() == 0)
            {
                return NotFound();
            }
            return Ok(entries);
            //return await _context.Entries.ToListAsync();
        }

        // GET: /Entries/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Entry>> GetEntry(int id)
        {
            var entries = _context.Entries.Include(x => x.PropertyPrice)
                .Include(x => x.PropertyDetails).Include(x => x.PropertyAddress).Include(x => x.PropertyFeatures)
                .Include(x => x.OfferDetails).ThenInclude(x => x.SellerContact)
                .FirstOrDefault(x => x.ID == id);

            if (entries == null)
            {
                return NotFound();
            }

            return Ok(entries);
        }

        // PUT: /Entries/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEntry(int id, Entry entry)
        {
            if (id != entry.ID)
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

        // POST: /Entries
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost("page")]
        public async Task<ActionResult<Entry>> PostEntry([FromBody]int pageNumber)
        {
            if (pageNumber <= 0)
            {
                return BadRequest();
            }

            var dobryAdres = new DobryAdresIntegration(null, null);
            var dump = dobryAdres.GenerateDumpOfPage(pageNumber);

            var entries = dump.Entries;
            if (entries.Count() == 0)
            {
                return NotFound();
            }
            _context.Entries.AddRange(entries);
            _context.SaveChanges();

            return Ok(entries);
            /*_context.Entries.Add(entry);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEntry", new { id = entry.ID }, entry);*/
        }

        // DELETE: /Entries/5
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
            return _context.Entries.Any(e => e.ID == id);
        }
    }
}
