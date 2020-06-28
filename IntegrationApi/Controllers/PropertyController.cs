using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatabaseConnection;
using IntegrationApi.Models;
using Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;

namespace IntegrationApi.Controllers
{
    [Route("")]
    [ApiController]
    public class PropertyController : ControllerBase
    {
        private readonly PFinderContext _context;
        //Elegancko było by to wstrzyknąć jako dowolną integracje, ale że nie możemy modyfikować interface-u,
        //a IWebSiteIntegration nie ma potrzebnych funkcji (do ładowania poszczególnych stron) ¯\_(ツ)_/¯
        private readonly SprzedawaczIntegration.SprzedawaczIntegration _integration;
        private const int pageSize = 20;

        public PropertyController(PFinderContext context)
        {
            _context = context;
            _integration = new SprzedawaczIntegration.SprzedawaczIntegration();
        }

        [Route("{*url}", Order = 999)]
        public ActionResult<string> GetHome() => new NotFoundObjectResult("Nothing's here");

        [HttpGet("/info")]
        public ActionResult<InfoResponseModel> GetInfo() => new InfoResponseModel
        {
            ConnectionString = PFinderContext.connectionString,
            IntegrationName = "sprzedawacz.pl",
            StudentName = "Maciej",
            StudentIndex = 160519
        };

        [HttpPost("/page")]
        public async Task<ActionResult> Post([FromBody]PageRequest value)
        {
            var entries = _integration.GeneratePage(value.PageNumber, pageSize);
            if (entries == null)
                return new NotFoundResult();
            await _context.AddRangeAsync(entries);
            _context.SaveChanges();
            return new NoContentResult();
        }

        [HttpGet("/entries")]
        public async Task<ActionResult<IEnumerable<Entry>>> GetEntries([FromQuery(Name = "PageLimit")] int? pageLimit, [FromQuery(Name = "PageId")] int? pageId)
        {
            List<Entry> entries;
            if(pageLimit != null && pageId != null)
                entries = await _context.Entries.Skip((int)pageLimit*(int)pageId).Take((int)pageLimit).ToListAsync();
            else
                entries = await _context.Entries.ToListAsync();
            if (!entries.Any())
                return new NotFoundResult();
            return entries;
        }

        [HttpGet("/entries/{id}")]
        public async Task<ActionResult<Entry>> GetEntry(int id)
        {
            var entry = await _context.Entries
                .Include(e => e.OfferDetails)
                .Include(e => e.PropertyAddress)
                .Include(e => e.PropertyDetails)
                .Include(e => e.PropertyFeatures)
                .Include(e => e.PropertyPrice)
                .SingleOrDefaultAsync(e => e.Id == id);

            if (entry == null)
            {
                return NotFound();
            }

            return entry;
        }

        [HttpPut("/entries/{id}")]
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
                if (!_context.Entries.Any(e => e.Id == id))
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
        /// <summary>
        /// Zwraca po 5 ofert z każdej kategorii o najniższej cenie na metr, dla miasta podanego w adresie
        /// </summary>
        /// <returns></returns>
        [HttpGet("/entries/best/{city}")]
        public async Task<ActionResult<IEnumerable<Entry>>> GetBestEntries(string city)
        {
            List<Entry> entries;
            PolishCity miasto;
            if (!Enum.TryParse(city.ToUpper(), out miasto))
                return new BadRequestResult();
            entries = _context.Entries.Where(e => e.PropertyAddress.City == miasto)
                .GroupBy(e => e.OfferDetails.OfferKind)
                .SelectMany(g => g.OrderBy(a => a.PropertyPrice.PricePerMeter).Take(5))
                .ToList();
            if (!entries.Any())
                return new NotFoundResult();
            return entries;
        }
    }
}