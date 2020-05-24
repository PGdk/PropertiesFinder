using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DatabaseConnection;
using Models;

namespace IntegrationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EntryIdsController : ControllerBase
    {
        private readonly BazosContext _context;

        public EntryIdsController(BazosContext context)
        {
            _context = context;
        }

        // GET: api/EntryIds
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EntryId>>> GetEntries()
        {
            return await _context.Entries.ToListAsync();
        }

        // GET: api/EntryIds/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EntryId>> GetEntryId(int id)
        {
            var entryId = await _context.Entries.FindAsync(id);

            if (entryId == null)
            {
                return NotFound();
            }

            return entryId;
        }

        // PUT: api/EntryIds/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEntryId(int id, EntryId entryId)
        {
            if (id != entryId.Id)
            {
                return BadRequest();
            }

            _context.Entry(entryId).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EntryIdExists(id))
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

        // POST: api/EntryIds
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<EntryId>> PostEntryId(EntryId entryId)
        {
            _context.Entries.Add(entryId);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEntryId", new { id = entryId.Id }, entryId);
        }

        // DELETE: api/EntryIds/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<EntryId>> DeleteEntryId(int id)
        {
            var entryId = await _context.Entries.FindAsync(id);
            if (entryId == null)
            {
                return NotFound();
            }

            _context.Entries.Remove(entryId);
            await _context.SaveChangesAsync();

            return entryId;
        }

        private bool EntryIdExists(int id)
        {
            return _context.Entries.Any(e => e.Id == id);
        }
    }
}
