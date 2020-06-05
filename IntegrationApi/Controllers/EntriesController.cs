using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DatabaseConnection;
using Models;
using System.Linq;

namespace IntegrationApi.Controllers
{
    [ApiController]
    public class EntriesController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public EntriesController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: /entries
        [Route("entries")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Entry>>> GetEntries()
        {
            return await _context.Entries
                .Include(e => e.OfferDetails)
                .ThenInclude(of => of.SellerContact)
                .Include(e => e.PropertyPrice)
                .Include(e => e.PropertyDetails)
                .Include(e => e.PropertyAddress)
                .Include(e => e.PropertyFeatures)
                .ToListAsync();
        }

        // GET: /entries/10/2
        [Route("entries/{pageLimit}/{pageId}")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Entry>>> GetEntriesPage(int pageLimit, int pageId)
        {
            if (pageLimit < 1 || pageId < 1)
            {
                return NotFound();
            }

            return await _context.Entries
                .Skip(pageLimit * (pageId - 1))
                .Take(pageLimit)
                .Include(e => e.OfferDetails)
                .ThenInclude(of => of.SellerContact)
                .Include(e => e.PropertyPrice)
                .Include(e => e.PropertyDetails)
                .Include(e => e.PropertyAddress)
                .Include(e => e.PropertyFeatures)
                .ToListAsync();
        }

        // GET: /entry/5
        [Route("entry/{id}")]
        [HttpGet]
        public async Task<ActionResult<Entry>> GetEntry(int id)
        {
            var entry = await _context.Entries
                .Include(e => e.OfferDetails)
                .ThenInclude(of => of.SellerContact)
                .Include(e => e.PropertyPrice)
                .Include(e => e.PropertyDetails)
                .Include(e => e.PropertyAddress)
                .Include(e => e.PropertyFeatures)
                .SingleOrDefaultAsync(e => id == e.Id);

            if (null == entry)
            {
                return NotFound();
            }

            return entry;
        }

        // PUT: /entry/5
        [Route("entry/{id}")]
        [HttpPut]
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
                if (EntryExists(id))
                {
                    throw;
                }

                return NotFound();
            }

            return NoContent();
        }

        private bool EntryExists(int id)
        {
            return _context.Entries.Any(e => id == e.Id);
        }
    }
}
