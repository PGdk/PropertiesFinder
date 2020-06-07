using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DatabaseConnection;
using Models;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace IntegrationApi.Controllers
{
    [ApiController]
    public class EntriesController : ControllerBase
    {
        private static readonly string PageLimitQueryParameterName = "pageLimit";
        private static readonly string PageIdQueryParameterName = "pageId";

        private readonly DatabaseContext _context;

        public EntriesController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: /entries || /entries/10/2
        [Route("entries")]
        [Authorize(Policy = "User")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Entry>>> GetEntries()
        {
            int? pageLimit = Request.Query.ContainsKey(PageLimitQueryParameterName)
                ? int.Parse(Request.Query[PageLimitQueryParameterName])
                : (int?) null;
            int? pageId = Request.Query.ContainsKey(PageIdQueryParameterName)
                ? int.Parse(Request.Query[PageIdQueryParameterName])
                : (int?) null;

            if ((null != pageLimit && null == pageId) || (null == pageLimit && null != pageId))
            {
                return BadRequest();
            }

            if (null != pageLimit && pageLimit < 1)
            {
                return BadRequest();
            }

            if (null != pageId && (pageId < 1 || (_context.Entries.Count() / pageLimit) < (pageId - 1)))
            {
                return NotFound();
            }

            return await _context.Entries
                .Skip((pageLimit * (pageId - 1)) ?? 0)
                .Take(pageLimit ?? _context.Entries.Count())
                .Include(e => e.OfferDetails)
                .ThenInclude(of => of.SellerContact)
                .Include(e => e.PropertyPrice)
                .Include(e => e.PropertyDetails)
                .Include(e => e.PropertyAddress)
                .Include(e => e.PropertyFeatures)
                .ToListAsync();
        }

        // GET: /entry/5
        [Route("entry/{id:int}")]
        [Authorize(Policy = "User")]
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
        [Route("entry/{id:int}")]
        [Authorize(Policy = "Admin")]
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
