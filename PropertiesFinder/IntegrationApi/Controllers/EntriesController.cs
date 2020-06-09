using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DatabaseConnection;
using Models;
using Microsoft.AspNetCore.Authorization;

namespace IntegrationApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    //[Authorize]
    public class EntriesController : ControllerBase
    {

     // GET: /entries
     /*
    [HttpGet]
        public IActionResult Get()
        //public async Task<ActionResult<IEnumerable<Entry>>> Get()
        {
            using DatabaseContext _context = new DatabaseContext();

            var entries = _context.Entries
            .Include(entry => entry.OfferDetails)
            .ThenInclude(offerDetails => offerDetails.SellerContact)
            .Include(entry => entry.PropertyAddress)
            .Include(entry => entry.PropertyDetails)
            .Include(entry => entry.PropertyFeatures)
            .Include(entry => entry.PropertyPrice).ToList();

            if (entries != null)
            {
                return Ok(entries);
            }
            else
            {
                return NotFound();   //404
            }
        }
        */

        //UZUPEŁNIENIE - STRONICOWANIE

        // GET: /entries
        [HttpGet]
        public ActionResult GetWithPaging(Page page)
        {
            using DatabaseContext _context = new DatabaseContext();

             var entries = _context.Entries
            .Skip(page.pageID) // pobiera z body jsona
            .Take(page.pageLimit)
            .Include(entry => entry.OfferDetails)
            .ThenInclude(offerDetails => offerDetails.SellerContact)
            .Include(entry => entry.PropertyAddress)
            .Include(entry => entry.PropertyDetails)
            .Include(entry => entry.PropertyFeatures)
            .Include(entry => entry.PropertyPrice).ToList();

            if (entries != null)
            {
                return Ok(entries);
            }
            else
            {
                return NotFound();   //404
            }
        }

        //GET: /entries/{id}
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            using DatabaseContext _context = new DatabaseContext();

            var entries = _context.Entries
            .Include(entry => entry.OfferDetails)
            .ThenInclude(offerDetails => offerDetails.SellerContact)
            .Include(entry => entry.PropertyAddress)
            .Include(entry => entry.PropertyDetails)
            .Include(entry => entry.PropertyFeatures)
            .Include(entry => entry.PropertyPrice).ToList();

            var entry = entries.FirstOrDefault(e => e.ID == id);

            if (entry == null)
            {
                return NotFound();
            }

            return Ok(entry);

        }

        //private bool EntryExists(int id)
        //{
        //    using DatabaseContext databaseContext = new DatabaseContext(); // ?
        //    return databaseContext.Entries.Any(e => e.ID == id);
        //}
    }
}
