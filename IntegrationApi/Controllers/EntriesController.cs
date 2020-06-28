using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatabaseConnection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;

namespace IntegrationApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class EntriesController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get(int pageLimit = -1, int pageId = -1)
        {
            if (pageLimit == -1 || pageId == -1)
                return Ok(DatabaseManager.GetAllEntries());

            int startingEntry = pageLimit * (pageId - 1);
            var entries = DatabaseManager.GetEntriesForGivenRange(startingEntry, pageLimit);

            return Ok(entries);
        }


        // Przykładowe zapytanie:
        // https://localhost:44362/api/entries/BestEntries/?city=warszawa&offerkind=rental&maximumprice=3000
        [HttpGet]
        public IActionResult BestEntries(string city, string offerKind, int maximumPrice)
        {
            var _city = ParseToPolishCity(city);
            var _offerKind = ParseToOfferKind(offerKind);

            if (_city == null
                || _offerKind == null
                || maximumPrice < 0)
            {
                return BadRequest();
            }

            var entries = DatabaseManager.GetAllEntries();
            var bestEntryPicker = new BestEntryPicker(entries, _city.Value, _offerKind.Value, maximumPrice);
            var bestEntries = bestEntryPicker.GetBestFiveEntries();

            return Ok(bestEntries);
        }

        private PolishCity? ParseToPolishCity(string phrase)
        {
            if (Enum.IsDefined(typeof(PolishCity), phrase.ToUpper()))
            {
                return (PolishCity)Enum.Parse(typeof(PolishCity), phrase.ToUpper());
            }
            return null;
        }

        private OfferKind? ParseToOfferKind(string phrase)
        {
            if (Enum.IsDefined(typeof(OfferKind), phrase.ToUpper()))
            {
                return (OfferKind)Enum.Parse(typeof(OfferKind), phrase.ToUpper());
            }
            return null;
        }

    }
}