using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatabaseConnection;
using DatabaseConnection.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace IntegrationApi.Controllers
{
    [Route("")]
    [ApiController]
    public class EntriesController : ControllerBase
    {
        private readonly DatabaseContext myDatabase;

        public EntriesController(DatabaseContext database)
        {
            myDatabase = database;
        }

        // GET: entries
        [HttpGet]
        [Route("entries")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<Entry>>> GetEntries()
        {
            if (Request.Headers.ContainsKey("X-Request-ID"))
            {
                myDatabase.Logs.Add(new Logs()
                {
                    date = DateTime.Now,
                    xRequestID = Request.Headers["X-Request-ID"]
                });
                myDatabase.SaveChanges();
            }
            var allEntries = await myDatabase.Entries
            .Include(e => e.OfferDetails)
                .ThenInclude(offerDetails => offerDetails.SellerContact)
            .Include(e => e.PropertyPrice)
            .Include(e => e.PropertyDetails)
            .Include(e => e.PropertyAddress)
            .Include(e => e.PropertyFeatures)
            .ToListAsync();

            if (allEntries != null)
            {
                return Ok(allEntries);
            }
            else
            {
                return NotFound($"There are no Entries");
            }
        }


        // GET: entries with Pagination
        [HttpGet]
        [Route("entries/pageId/{pageId}/PageLimit/{pageLimit}/")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<Entry>>> GetEntriesWithPagination(int ?pageId, int ?pageLimit)
        {
            if (Request.Headers.ContainsKey("X-Request-ID"))
            {
                myDatabase.Logs.Add(new Logs()
                {
                    date = DateTime.Now,
                    xRequestID = Request.Headers["X-Request-ID"]
                });
                myDatabase.SaveChanges();
            }

            if (pageId == null || pageLimit == null)

            { return BadRequest("Nalezy podac poprawne pageId i pageLimit"); }
            if (pageId < 1)
            {
                return BadRequest("PageId musi byc wieksze od zera");
            }

            var allEntriesWithPagination = await myDatabase.Entries
            .Where(e => e.Id > (pageId - 1) * pageLimit && e.Id <= ((pageId - 1) * pageLimit + pageLimit))
                .Include(e => e.OfferDetails)
                    .ThenInclude(offerDetails => offerDetails.SellerContact)
                .Include(e => e.PropertyPrice)
                .Include(e => e.PropertyDetails)
                .Include(e => e.PropertyAddress)
                .Include(e => e.PropertyFeatures)
                .ToListAsync();

            if (allEntriesWithPagination != null)
            {
                return Ok(allEntriesWithPagination);
            }
            else
            {
                return NotFound($"There are no Entries");
            }
        }

        // GET entry/5
        [Route("entry/{id}")]
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<Entry>>> GetEntry(int id)
        {
            if (Request.Headers.ContainsKey("X-Request-ID"))
            {
                myDatabase.Logs.Add(new Logs()
                {
                    date = DateTime.Now,
                    xRequestID = Request.Headers["X-Request-ID"]
                });
                myDatabase.SaveChanges();
            }
            var searchedEntry = await myDatabase.Entries
             .Include(e => e.OfferDetails)
                 .ThenInclude(offerDetails => offerDetails.SellerContact)
             .Include(e => e.PropertyPrice)
             .Include(e => e.PropertyDetails)
             .Include(e => e.PropertyAddress)
             .Include(e => e.PropertyFeatures)
             .FirstOrDefaultAsync(e => e.Id == id);

            if (searchedEntry != null)
            {
                return Ok(searchedEntry);
            }
            else
            {
                return NotFound($"There is no entry with id {id}");
            }
        }

        // Put entry/5
        [Route("entry/{id}")]
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<Entry>>> UpdateEntry(int entryId, Entry entry)
        {
            if (Request.Headers.ContainsKey("X-Request-ID"))
            {
                myDatabase.Logs.Add(new Logs()
                {
                    date = DateTime.Now,
                    xRequestID = Request.Headers["X-Request-ID"]
                });
                myDatabase.SaveChanges();
            }

            var updatedEntry = await myDatabase.Entries.FirstOrDefaultAsync(e => e.Id == entryId);

            if (updatedEntry != null)
            {
                myDatabase.Update(entry);
                myDatabase.SaveChanges();
                return Ok("Entry Updated");
            }
            else
            {
                return NotFound($"There is no entry to update with id {entryId}");
            }
        }

        // POST
        [Route("pageNumber/{pageId}")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult PostPage(int pageId)
        {
            if (Request.Headers.ContainsKey("X-Request-ID"))
            {
                myDatabase.Logs.Add(new Logs()
                {
                    date = DateTime.Now,
                    xRequestID = Request.Headers["X-Request-ID"]
                });

                myDatabase.SaveChanges();
            }

            var parseOnePage = new SampleApp.ParseOnePage(pageId);
            var ListToPrint =  parseOnePage.GetListToPrint();
            return Ok(ListToPrint);
        }
    }
}
