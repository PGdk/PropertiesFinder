using System;
using System.Collections.Generic;
using System.Linq;
using Application.Trovit;
using DatabaseConnection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Models;

namespace IntegrationApi.Controllers
{

    public class AddPageRequest 
    {
        public int PageNumber { get; set; }
    }
    
    [ApiController]
    [Route("")]
    public class SprzedajemyController : Controller
    {
        private ITrovitRepository repository;
        private TrovitIntegration integration;
        private ITrovitService service;

        public SprzedajemyController(ITrovitRepository repository, ITrovitService service, IDumpsRepository dump, IEqualityComparer<Entry> compare)
        {
            this.repository = repository;
            this.integration = new TrovitIntegration(dump, compare);
            this.service = service;
        }

        [HttpPost]
        [Route("page")]
        public IActionResult AddPage(AddPageRequest request)
        {
            if (request.PageNumber < 1)
            {
                return new BadRequestObjectResult("invalid page number");
            }

            var entries = integration.GetOffersByPage(request.PageNumber);

            if (entries.Count() == 0)
            {
                return new NotFoundResult();
            }

            this.repository.AddEntries(entries);

            return new NoContentResult();
        }

        [HttpGet]
        [Route("entries")]
        public IActionResult GetEntries(string pageLimit, string pageId)
        {
            if (pageLimit == null || pageId == null)
            {
                return new OkObjectResult(repository.GetEntries());
            }
            try
            {
                var limit = int.Parse(pageLimit);
                var page = int.Parse(pageId);

                if (page < 1)
                {
                    return new BadRequestObjectResult("invalid payload");
                }

                return new OkObjectResult(repository.GetEntries(page, limit));
            }
            catch (Exception)
            {
                return new BadRequestObjectResult("invalid payload format");
            }
        }

        [HttpGet]
        [Route("entries/{entryId}")]
        public IActionResult Get(int entryId)
        {
            var result = repository.GetEntry(entryId);

            if (result == null)
            {
                return new NotFoundObjectResult("not found");
            }
            return new OkObjectResult(result);
        }

        [HttpGet]
        [Route("entries/top")]
        public IActionResult GetSpecialOffers()
        {
            var top = this.service.GetTopOffers();

            if (top == null)
                return new NotFoundResult();

            return new OkObjectResult(top);
        }
    }
}