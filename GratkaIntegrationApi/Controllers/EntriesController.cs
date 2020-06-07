using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DatabaseConnection;
using Models;
using Application.Gratka;
using GratkaIntegrationApi.Models;
namespace GratkaIntegrationApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class EntriesController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly IDumpsRepository _dumpsRepository;
        private readonly IEqualityComparer<Entry> _equalityComparer;
        private readonly GratkaIntegration _gratka;
        public EntriesController(DatabaseContext context, IDumpsRepository dumpsRepository, IEqualityComparer<Entry> equalityComparer)
        {
            _context = context;
            _dumpsRepository = dumpsRepository;
            _equalityComparer = equalityComparer;
            _gratka = new GratkaIntegration(_dumpsRepository, _equalityComparer);
        }
        // GET: api/Entries
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Entry>>> GetEntries(string pageLimit, string pageId)
        {
            List<Entry> entries;
            if (pageLimit == null && pageId == null)
            {
                entries = await _context.Entries
               .Include(e => e.OfferDetails)
                   .ThenInclude(offerDetails => offerDetails.SellerContact)
               .Include(e => e.PropertyAddress)
               .Include(e => e.PropertyDetails)
               .Include(e => e.PropertyFeatures)
               .Include(e => e.PropertyPrice)
               .ToListAsync();
                if (entries.Count() == 0)
                {
                    NotFound();
                }
                return Ok(entries);
            }
            else if (pageLimit != null && pageId != null)
            {
                int parsedPageLimit, parsedPageId;
                try
                {
                    parsedPageLimit = int.Parse(pageLimit);
                    parsedPageId = int.Parse(pageId);
                    if (parsedPageLimit < 1 | parsedPageId < 1)
                    {
                        return BadRequest("Pagelimit and pageId have to be integers.");
                    }
                }
                catch (Exception)
                {
                    return BadRequest("Pagelimit and pageId have to be integers.");
                }
                var offset = parsedPageLimit * parsedPageId - parsedPageLimit;
                entries = await _context.Entries.Skip(offset).Take(parsedPageLimit)
               .Include(e => e.OfferDetails)
                   .ThenInclude(offerDetails => offerDetails.SellerContact)
               .Include(e => e.PropertyAddress)
               .Include(e => e.PropertyDetails)
               .Include(e => e.PropertyPrice)
               .Include(e => e.PropertyFeatures)
               .ToListAsync();
                if (entries.Count() == 0)
                {
                    NoContent();
                }
                return Ok(entries);
            }
            else
            {
                return BadRequest("Please add pageLimit&pageId.");
            }
        }
        // GET: api/Entries/5
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<Entry>>> GetEntry(int id)
        {
            var entry = await _context.Entries.Where(e => e.ID == id)
                .Include(e => e.OfferDetails)
                    .ThenInclude(offerDetails => offerDetails.SellerContact)
                .Include(e => e.PropertyAddress)
                .Include(e => e.PropertyPrice)
                .Include(e => e.PropertyFeatures)
                .Include(e => e.PropertyDetails)
                .ToListAsync();
            if (entry == null)
            {
                return NotFound();
            }
            return entry;
        }
        // PUT: api/Entries/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEntry(int id, Entry entry)
        {
            if (id != entry.ID)
            {
                return BadRequest();
            }
            _context.Update(entry);
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
        // POST: api/Entries
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Entry>> PostEntry(PostRequest req)
        {
            var entries = _gratka.GenerateEntriesFromPage(req.PageNumber);
            if (entries.Count == 0)
            {
                return NotFound();
            }
            _context.Entries.AddRange(entries);
            await _context.SaveChangesAsync();
            return Ok(entries);
        }
        // DELETE: api/Entries/5
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
