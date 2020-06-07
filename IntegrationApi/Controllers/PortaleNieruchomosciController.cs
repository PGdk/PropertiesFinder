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

        [Route("entry/{id}")]
        [HttpGet]
        public ActionResult<Entry> SingleEntry(int id)
        {
            var entries = db.Entries.Include("OfferDetails").Include("PropertyPrice")
                .Include("PropertyDetails").Include("PropertyAddress").Include("PropertyFeatures")
                .FirstOrDefault(x => x.ID == id);

            if (entries == null)
            {
                return NotFound();
            }

            return Ok(entries);
        }
        [Route("entry/{id}")]
        [HttpPut]
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

        [Route("entries")]
        [HttpGet]
        public ActionResult<IEnumerable<Entry>> AllEntries()
        {
            var entries = db.Entries.Include("OfferDetails").Include("PropertyPrice")
                .Include("PropertyDetails").Include("PropertyAddress").Include("PropertyFeatures").ToList();
            if (entries.Count() == 0)
            {
                return NotFound();
            }
            return Ok(entries);
        }

        [Route("entries/{pagelimit}/{pageid}")]
        [HttpGet]
        public IActionResult GetLimitId(int pageLimit, int pageId)
        {
            if(pageLimit <= 0 || pageId <= 0)
            {
                return BadRequest();
            }

            var entries = db.Entries.Include("OfferDetails").Include("PropertyPrice")
                .Include("PropertyDetails").Include("PropertyAddress").Include("PropertyFeatures").ToList()
                .Skip((pageId - 1) * pageLimit).Take(pageLimit);

            if (entries.Count() == 0)
            {
                return NotFound();
            }

            return Ok(entries);
        }
        
        [Route("page")]
        [HttpPost]
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