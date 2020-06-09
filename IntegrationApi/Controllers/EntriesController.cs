using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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
        private readonly DatabaseContext _context;

        public EntriesController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: api/Entries
        [HttpGet]
        public IActionResult GetEntries([FromQuery(Name = "PageLimit")] int? PageLimit, [FromQuery(Name = "PageId")] int? PageId)
        {
            var entry = _context.Entries
                .Include(e => e.OfferDetails)
                    .ThenInclude(od => od.SellerContact)
                .Include(e => e.PropertyAddress)
                .Include(e => e.PropertyDetails)
                .Include(e => e.PropertyPrice)
                .Include(e => e.PropertyFeatures);

            if(PageLimit != null && PageId != null)
            {
                PageId--;
                var temp = entry.Skip((int)PageId * (int)PageLimit).Take((int)PageLimit).ToList());
                return Ok(temp);
            }
            else
            {
                return Ok(entry.ToList());
            }
        }

        // GET: api/Entries/5
        [HttpGet("{id}")]
        public IActionResult GetEntry(int id)
        {
            var entry = _context.Entries
                .Include(e => e.OfferDetails)
                    .ThenInclude(od => od.SellerContact)
                .Include(e => e.PropertyAddress)
                .Include(e => e.PropertyDetails)
                .Include(e => e.PropertyPrice)
                .Include(e => e.PropertyFeatures)
                .Where(e => e.ID == id)
                .FirstOrDefault();

            if (entry == null)
            {
                return NotFound();
            }

            return Ok(entry);
        }

        // PUT: api/Entries/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public IActionResult PutEntry(int id, Entry entry)
        {
            if (id != entry.ID)
            {
                return BadRequest();
            }

            _context.Entry(entry).State = EntityState.Modified;

            _context.Entries.Update(entry);
            _context.SaveChanges();
            return NoContent();
        }

        // POST: api/Entries
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public IActionResult PostEntry(Entry entry)
        {
            _context.Entries.Add(entry);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetEntry), new { id = entry.ID }, entry);
        }

        // DELETE: api/Entries/5
        [HttpDelete("{id}")]
        public IActionResult DeleteEntry(int id)
        {
            var entry = _context.Entries
                .Include(e => e.OfferDetails)
                    .ThenInclude(od => od.SellerContact)
                .Include(e => e.PropertyAddress)
                .Include(e => e.PropertyDetails)
                .Include(e => e.PropertyPrice)
                .Include(e => e.PropertyFeatures)
                .Where(e => e.ID == id)
                .FirstOrDefault();
            if (entry == null)
            {
                return NotFound();
            }

            _context.Entries.Remove(entry);
            _context.SaveChanges();
            return Ok(entry);
        }

        private bool EntryExists(int id)
        {
            return _context.Entries.Any(e => e.ID == id);
        }
    }
}
