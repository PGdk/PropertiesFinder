using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DatabaseConnection;
using Models;
using Morizon;
using Utilities;
using System.Net;
using System;
using System.Text.Json;
using Newtonsoft.Json.Linq;

namespace IntegrationApi.Controllers
{
    [Route("")]
    [ApiController]
    public class EntriesController : ControllerBase
    {
        private readonly MorizonContext _context;

        public EntriesController(MorizonContext context)
        {
            _context = context;
        }

        // GET Entries?PageId=5&PageLimit=50
        [HttpGet]
        [Route("Entries")]
        public async Task<ActionResult<IEnumerable<Entry>>> GetEntries(int pageId = 1, int pageLimit = 20)
        {
            int skip = ( pageId - 1 ) * pageLimit;
                
             var entries =  await _context.Entries
                .Include(entry => entry.OfferDetails)
                    .ThenInclude(offerDetails => offerDetails.SellerContact)
                .Include(entry => entry.PropertyAddress)
                .Include(entry => entry.PropertyDetails)
                .Include(entry => entry.PropertyFeatures)
                .Include(entry => entry.PropertyPrice)
                .Skip(skip)
                .Take(pageLimit)
                .ToListAsync();

            return entries;
        }

        // GET: api/Entry/5
        [HttpGet("{id}")]
        [Route("Entry/{id}")]
        public async Task<ActionResult<Entry>> GetEntry(int id)
        {
            var entry = await _context.Entries.FindAsync(id);

            if (entry == null)
            {
                return NotFound();
            }

            await _context.Entry(entry).Reference(entry => entry.PropertyAddress).LoadAsync();
            await _context.Entry(entry).Reference(entry => entry.PropertyDetails).LoadAsync();
            await _context.Entry(entry).Reference(entry => entry.PropertyFeatures).LoadAsync();
            await _context.Entry(entry).Reference(entry => entry.PropertyPrice).LoadAsync();

            return entry;
        }

        // PUT: UpdateEntry/5
        [HttpPut("{id}")]
        [Route("UpdateEntry/{id}")]
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

        // POST: page
        [HttpPost]
        [Route("Page")]
        public async Task<ActionResult<IEnumerable<Entry>>> PostPage([FromBody] JsonElement body)
        {

            int pageNumber;
            try {
                pageNumber =int.Parse(body.GetProperty("page").ToString());
                WebClient wc = new WebClient();
                string HTMLSource = wc.DownloadString("https://www.morizon.pl/mieszkania/" + pageNumber);
            }
            catch ( Exception ) {
                return BadRequest("The page number provided is invalid");
            }

            MorizonIntegration Morizon = new MorizonIntegration(new DumpFileRepository(), new MorizonComparer());
            List<Entry> entries = Morizon.GetPage(pageNumber);

            foreach ( Entry entry in entries )
                _context.Entries.Add(entry);

            await _context.SaveChangesAsync();

            return entries;
        }

        // DELETE: DeleteEntry/5
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
            return _context.Entries.Any(e => e.ID == id);
        }


        // GET: 
        [HttpGet]
        [Route("Info")]
        public JsonResult GetInfo() {
            Dictionary<string, string> output = new Dictionary<string, string>();

            output.Add("connectionString", @"Data Source=.\SQLEXPRESS;Initial Catalog=joannal165459;Integrated Security=True");
            output.Add("integrationName", "morizon");
            output.Add("studentName", "Joanna");
            output.Add("studentIndex", "165459");

            return new JsonResult(output);
        }

    }
}
