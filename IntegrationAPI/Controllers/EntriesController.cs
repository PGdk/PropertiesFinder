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
using Newtonsoft.Json.Linq;
using DatabaseConnection.Models;

namespace IntegrationAPI.Controllers
{
    [Route("")]
    [ApiController]
    public class EntriesController : ControllerBase
    {
        private readonly BazosContext _context;

        public EntriesController(BazosContext context)
        {
            _context = context;
        }

        // GET: /Entries
        [HttpGet]
        // DODATKOWE - autoryzacja (patrz również na inne metody)
        [Authorize(Policy = "User")]
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

        // Projekt 3 Podpunkt 1
        // GET: /Entries/Best
        [HttpGet]
        [Authorize(Policy = "User")]
        [Route("Entries/Best")]
        public async Task<ActionResult<IEnumerable<Entry>>> GetBestEntries()
        {
            var entries = await _context.Entries
                            .Include(entry => entry.OfferDetails)
                                .ThenInclude(offerDetails => offerDetails.SellerContact)
                            .Include(entry => entry.PropertyAddress)
                            .Include(entry => entry.PropertyDetails)
                            .Include(entry => entry.PropertyFeatures)
                            .Include(entry => entry.PropertyPrice)
                            .OrderBy(entry => entry.PropertyPrice.TotalGrossPrice)
                            .ToListAsync();
            if (entries == null)
            {
                // Dodatkowe - Zwrot 404
                return NotFound();
            }
            else
            {
                var bestEntries = Bazos.BestOffersFinder.GetBestOffers(entries);
                return bestEntries;
            }
        }

        // DODATKOWE: PageLimit i PageId
        // GET: /Entries/20/2
        [HttpGet("{pageLimit}/{pageId}")]
        [Authorize(Policy = "User")]
        [Route("Entries/{pageLimit}/{pageId}")]
        public async Task<ActionResult<IEnumerable<Entry>>> GetEntriesFromPage(int pageLimit, int pageId)
        {
            int numberToSkip = pageLimit * (pageId - 1);
            var entries = await _context.Entries
                            .Include(entry => entry.OfferDetails)
                                .ThenInclude(offerDetails => offerDetails.SellerContact)
                            .Include(entry => entry.PropertyAddress)
                            .Include(entry => entry.PropertyDetails)
                            .Include(entry => entry.PropertyFeatures)
                            .Include(entry => entry.PropertyPrice)
                            .OrderBy(entry => entry.Id)
                            .Skip(numberToSkip)
                            .Take(pageLimit)
                            .ToListAsync();
            if (entries == null)
            {
                // Dodatkowe - Zwrot 404
                return NotFound();
            }
            else
            {
                return entries;
            }

        }

        // GET: /Entry/5
        [HttpGet("{id}")]
        [Authorize(Policy = "User")]
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

            for (int i = 0; i < entryList.Count; i++)
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

        // DODATKOWE - aktualizowanie danych
        // PUT: Entry/5

        [HttpPut("{id}")]
        // DODATKOWE - autoryzacja User/Admin
        [Authorize(Policy = "Admin")]
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

        //// POST: /Entry/2
        //[HttpPost]
        //[Authorize(Policy = "User")]
        //[Route("Entry")]
        //public async Task<ActionResult<Entry>> PostEntry(Entry entry)
        //{
        //    _context.Entries.Add(entry);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction("GetEntry", new { id = entry.Id }, entry);
        //}

        // POST: Page/id
        [HttpPost("{id}")]
        [Authorize(Policy = "User")]
        [Route("Page/{id}")]
        public async Task<ActionResult<Entry>> PostPage(int id)
        {
            //Pobierz daną stronę z aplikacji
            var entries = Bazos.BazosIntegration.GenerateDump(id); 
            if (entries.Count == 0)
            {
                return BadRequest();
            }
            _context.Entries.AddRange(entries);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: Page (wyslanie numeru strony JSONem
        [HttpPost]
        [Authorize(Policy = "User")]
        [Route("Page")]
        public async Task<ActionResult<Entry>> PostPageJSon(Page page)
        {
            var entries = Bazos.BazosIntegration.GenerateDump(page.pageNumber);
            if (entries.Count == 0)
            {
                // Dodatkowe - Zwrot 400
                return BadRequest();
            }
            _context.Entries.AddRange(entries);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: /Entry/5
        [HttpDelete("{id}")]
        [Authorize(Policy = "Admin")]
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
