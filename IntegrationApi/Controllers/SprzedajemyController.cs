using System;
using System.Collections.Generic;
using System.Linq;
using DatabaseConnection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using IntegrationSprzedajemy;
using Models;
using Microsoft.AspNetCore.Authentication;
using IntegrationSprzedajemyService;

namespace IntegrationApi.Controllers
{
    [ApiController]
    [Route("")]
    public class SprzedajemyController : Controller
    {
        private readonly ISprzedajemyRepository databaseRepository;
        private readonly IDumpsRepository dumpsRepository;
        private readonly IEqualityComparer<Entry> equalityComparer;
        private readonly Integration integraionSprzedajemy;
        private readonly ISprzedajemyService sprzedajemyService;

        public SprzedajemyController(ISprzedajemyRepository databaseRepository, IDumpsRepository dumpsRepository, IEqualityComparer<Entry> equalityComparer, ISprzedajemyService sprzedajemyService)
        {
            this.databaseRepository = databaseRepository;
            this.dumpsRepository = dumpsRepository;
            this.equalityComparer = equalityComparer;
            this.integraionSprzedajemy = new Integration(this.dumpsRepository, this.equalityComparer);
            this.sprzedajemyService = sprzedajemyService;
        }

        [HttpGet]
        [Route("{entryId}")]
        [Authorize(Policy = "User")]
        public IActionResult Get(int entryId)
        {
            var result = databaseRepository.GetEntry(entryId);

            if (result == null)
            {
                return new NotFoundObjectResult("Entry does not exist!");
            }
            return new OkObjectResult(result);
        }

        [HttpPost]
        [Route("page")]
        [Authorize(Policy = "User")]
        public IActionResult AddPage(PageRequestModel page)
        {
            if (page.PageNumber < 1)
            {
                return new BadRequestObjectResult("pageId < 1!");
            }

            var entries = integraionSprzedajemy.GetOffersByPageNum(page.PageNumber);

            if (entries.Count() == 0)
            {
                return new NotFoundResult();
            }

            databaseRepository.AddEntries(entries);

            return new NoContentResult();
        }

        [HttpGet]
        [Route("entries")]
        [Authorize(Policy = "User")]
        public IActionResult GetEntries(string pageLimit, string pageId)
        {
            if (pageLimit != null && pageId != null)
            {
                try
                {
                    var limit = int.Parse(pageLimit);
                    var page = int.Parse(pageId);

                    if (page < 1)
                    {
                        return new BadRequestObjectResult("pageId < 1!");
                    }
                  
                    return new OkObjectResult(databaseRepository.GetEntries(page, limit));
                }
                catch (Exception)
                {
                    return new BadRequestObjectResult("Bad pageId or pageLimit!");
                }
            }

            if (pageLimit == null ^ pageId == null)
            {
                return new BadRequestObjectResult("pageId XOR pageLimit are needed!");
            }

            return new OkObjectResult(databaseRepository.GetEntries());
        }

        [HttpPut]
        [Route("entries/{id}")]
        [Authorize(Policy = "Admin")]
        public IActionResult UpdateEntry(int id, Entry entry)
        {
            var entryUpdated = databaseRepository.UpdateEntry(id, entry);

            if (entryUpdated == null)
            {
                return new BadRequestObjectResult("Bad request");
            }

            return new OkObjectResult(entryUpdated);
        }

        [HttpGet]
        [Route("entries/specialOffers")]
        [Authorize(Policy = "User")]
        public IActionResult GetSpecialOffers()
        {
            var specialOffers = this.sprzedajemyService.GetSpecialOffers();

            if (specialOffers == null)
                return new NotFoundResult();

            return new OkObjectResult(specialOffers);
        }
    }
}