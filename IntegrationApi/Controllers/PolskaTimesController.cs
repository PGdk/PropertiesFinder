using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DatabaseConnection;
using Models;
using Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IntegrationApi.Controllers
{
    [Route("")]
    [ApiController]
    public class PolskaTimesController : ControllerBase
    {
        private readonly PolskaTimesDBContext _context;
        public PolskaTimesController(PolskaTimesDBContext context)
        {
            _context = context;
        }

        // GET: /info
        [HttpGet]
        [Route("Info")]
        public InfoStatus GetInfo()
        {
            return new InfoStatus()
            {
                ConnectionString = "Data Source=.;Initial Catalog=KacperR174335;Integrated Security=True",
                IntegrationName = "Polska Times",
                StudentName = "Kacper Ratyński",
                StudentIndex = 174335
            };
        }

        // GET: /entry/5
        [HttpGet]
        [Route("Entry/{id}")]
        public async Task<ActionResult<Entry>> GetEntry(int id)
        {
            if(id < 1)
            {
                return NotFound();
            }
            var entry = _context.entries.Where(e => e.ID == id).Include(e => e.OfferDetails)
                                    .ThenInclude(o => o.SellerContact)
                                    .Include(e => e.PropertyAddress)
                                    .Include(e => e.PropertyDetails)
                                    .Include(e => e.PropertyFeatures)
                                    .Include(e => e.PropertyPrice).FirstOrDefault();
            if(entry != null)
            {
                return entry;
            }
            return NotFound();
        }

        // GET: /entries
        [HttpGet]
        [Route("Entries")]
        public async Task<ActionResult<IEnumerable<Entry>>> GetEntries()
        {
            var entries = _context.entries.Include(e => e.OfferDetails)
                                    .ThenInclude(o => o.SellerContact)
                                    .Include(e => e.PropertyAddress)
                                    .Include(e => e.PropertyDetails)
                                    .Include(e => e.PropertyFeatures)
                                    .Include(e => e.PropertyPrice).ToList();
            if (entries != null)
            {
                return entries;
            }
            return NotFound();
        }

        // GET: /Entries/40/5
        [HttpGet]
        [Route("Entries/{PageLimit}/{PageId}")]
        public async Task<ActionResult<IEnumerable<Entry>>> GetSomeEntries(int PageLimit, int PageId)
        {
            var start = (PageId - 1) * PageLimit;

            var entries = _context.entries.Include(e => e.OfferDetails)
                                    .ThenInclude(o => o.SellerContact)
                                    .Include(e => e.PropertyAddress)
                                    .Include(e => e.PropertyDetails)
                                    .Include(e => e.PropertyFeatures)
                                    .Include(e => e.PropertyPrice).Skip(start).Take(PageLimit).OrderBy(e => e.ID).ToList();

            if (entries == null)
            {
                return NotFound();
            }
            return entries;
        }

        // POST: /Entry/2
        [HttpPost]
        [Route("Entry")]
        public async Task<ActionResult<Entry>> PostEntry(Entry entry)
        {
            _context.entries.Add(entry);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEntry", new { id = entry.ID }, entry);
        }

        // POST: Page/5
        [HttpPost]
        [Route("Page/{id}")]
        public async Task<ActionResult<Entry>> PostPage(int id)
        {
            if(id < 1)
            {
                return BadRequest();
            }
            var entries = PolskaTimes.Integration.GenerateDump(id);
            if (entries.Count == 0)
            {
                return BadRequest();
            }
            _context.entries.AddRange(entries);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut]
        [Route("UpdateEntry/")]
        public async Task<IActionResult> UpdateEntry(Entry entry)
        {
            if(entry.ID < 1)
            {
                return BadRequest();
            }

            var ent = _context.entries.Find(entry.ID);

            if (ent != null)
            {
                ent.OfferDetails = entry.OfferDetails;
                ent.PropertyAddress = entry.PropertyAddress;
                ent.PropertyDetails = entry.PropertyDetails;
                ent.PropertyFeatures = entry.PropertyFeatures;
                ent.PropertyPrice = entry.PropertyPrice;
                ent.RawDescription = entry.RawDescription;
            }
            _context.SaveChanges();

            if (ent == null)
            {
                return BadRequest();
            }
            return new OkResult();
        }
    }
}
