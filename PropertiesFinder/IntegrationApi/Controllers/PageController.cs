using Aplication;
using DatabaseConnection;
using Microsoft.AspNetCore.Mvc;
using Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;


namespace IntegrationAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    //[Authorize]
    public class PageController : ControllerBase
    {
        // POST: page
        [HttpPost]
        //public ActionResult Post(int id)  // tak dziala z ID
        public ActionResult Post(Page idPage)
        {
            using DatabaseContext databaseContext = new DatabaseContext();

            List<Entry> entries = new List<Entry>();
            //entries = DomiportaIntegration.GetEntriesPageID(id); // tak dziala z ID
            entries = DomiportaIntegration.GetEntriesPageID(idPage.pageNumber);

            if (entries.Count == 0)
            {
                return BadRequest();
            }            

            foreach (Entry entry in entries)
            {
                databaseContext.Entries.Add(entry);
            }

            databaseContext.SaveChanges();

            //PropertyPrice pp = new PropertyPrice
            //{
            //    PricePerMeter = 80,
            //    //ResidentalRent = 50,
            //    TotalGrossPrice = 1200
            //};
            //OfferDetails od = new OfferDetails
            //{
            //    Url = "adres www",
            //    OfferKind = OfferKind.SALE
            //};
            //Entry entry = new Entry { RawDescription = "Cokolwiek string 13", PropertyPrice = pp, OfferDetails = od };

            return NoContent();
        }

    }
}
