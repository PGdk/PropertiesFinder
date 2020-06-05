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
    }
}
