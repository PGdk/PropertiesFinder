using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using DatabaseConnectionNew;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Application.Portalenieruchomosci;
using Models;
using Microsoft.Net.Http.Headers;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;



namespace IntegrationApi.Controllers
{
    
    [Route("/")]
    [Authorize]
    [ApiController]
    public class PortaleNieruchomosciController : ControllerBase
    {
        private DatabaseContext db = new DatabaseContext();
        [Authorize(Policy = "User")]
        [HttpGet("entry/{id}")]
        public ActionResult<Entry> SingleEntry(int id)
        {
            var entries = db.Entries.Include(x => x.PropertyPrice)
                .Include(x => x.PropertyDetails).Include(x => x.PropertyAddress).Include(x => x.PropertyFeatures)
                .Include(x => x.OfferDetails).ThenInclude(x => x.SellerContact)
                .FirstOrDefault(x => x.ID == id);

            if (entries == null)
            {
                return NotFound();
            }

            return Ok(entries);
        }
        [Authorize(Policy = "Admin")]
        [HttpPut("entry/{id}")]
        public IActionResult PutKsiazka(int id, [FromBody]Entry entries)
        {
            if (id != entries.ID)
            {
                return BadRequest();
            }

            try
            {
                db.Update(entries);
                db.SaveChanges();
            }
            catch(DbUpdateException)
            {
                if (!EntryExists(id))
                {
                    return NotFound();
                }
                else
                {
                    return BadRequest();
                }
            }

            return NoContent();
        }
        [Authorize(Policy = "User")]
        [HttpGet("entries")]
        public ActionResult<IEnumerable<Entry>> AllEntries()
        {
            var entries = db.Entries.Include(x=>x.PropertyPrice)
                .Include(x=>x.PropertyDetails).Include(x=>x.PropertyAddress).Include(x=>x.PropertyFeatures)
                .Include(x => x.OfferDetails).ThenInclude(x => x.SellerContact).ToList();
            if (entries.Count() == 0)
            {
                return NotFound();
            }
            return Ok(entries);
        }

        [Authorize(Policy = "User")]
        [HttpGet("entries/{pagelimit}/{pageid}")]
        public IActionResult GetLimitId(int pageLimit, int pageId)
        {
            if(pageLimit <= 0 || pageId <= 0)
            {
                return BadRequest();
            }

            var entries = db.Entries.Include(x => x.PropertyPrice)
                .Include(x => x.PropertyDetails).Include(x => x.PropertyAddress).Include(x => x.PropertyFeatures)
                .Include(x => x.OfferDetails).ThenInclude(x => x.SellerContact).ToList()
                .Skip((pageId - 1) * pageLimit).Take(pageLimit);

            if (entries.Count() == 0)
            {
                return NotFound();
            }

            return Ok(entries);
        }

        [Authorize(Policy = "User")]
        [HttpPost("page")]
        public IActionResult PostEntries([FromBody]int pageNumber)
        {
            if(pageNumber <= 0)
            {
                return BadRequest();
            }

            var pn = new PortaleIntegration(null, null);
            var dump = pn.GenerateDump();

            var entries = dump.Entries.Skip((pageNumber - 1) * 10).Take(10);
            if(entries.Count() == 0)
            {
                return NotFound();
            }
            db.Entries.AddRange(entries);
            db.SaveChanges();

            return Ok(entries);
        }

        private bool EntryExists(int id)
        {
            return db.Entries.Any(e => e.ID == id);
        }
    }
}