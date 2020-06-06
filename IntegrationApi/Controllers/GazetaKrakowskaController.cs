using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using DatabaseConnection;
using DatabaseConnection.Models;
using GazetaKrakowska;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Models;

namespace IntegrationApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GazetaKrakowskaController : ControllerBase
    {
        private readonly IGazetaKrakowskaRepository databaseRepository;
        private readonly IMapper mapper;
        private readonly IDumpsRepository dumpsRepository;
        private readonly IEqualityComparer<Entry> equalityComparer;
        private readonly GazetaKrakowskaIntegration gk;

        public GazetaKrakowskaController(IGazetaKrakowskaRepository databaseRepository, IMapper mapper, IDumpsRepository dumpsRepository, IEqualityComparer<Entry> equalityComparer)
        {
            this.databaseRepository = databaseRepository;
            this.mapper = mapper;
            this.dumpsRepository = dumpsRepository;
            this.equalityComparer = equalityComparer;
            this.gk = new GazetaKrakowskaIntegration(this.dumpsRepository, this.equalityComparer);
        }

        [HttpGet]
        [Route("{entryId}")]
        [Authorize(Policy = "User")]
        public IActionResult Get(int entryId)
        {
            var result = this.databaseRepository.GetEntry(entryId);

            if (result == null)
                return new NotFoundObjectResult("Entry does not exist with given Id");
            return new OkObjectResult(result);
        }

        [HttpPost]
        [Route("page")]
        [Authorize(Policy = "User")]
        public IActionResult AddPage(PageRequest page)
        {
            if (page.PageNumber < 1)
                return new BadRequestObjectResult("Page number cannot be less than 1");

            var entries = gk.FetchOfferFromGivenPage(page.PageNumber);

            if(entries.Count() == 0)
                return new NotFoundResult();

            var entriesDb = entries.Select(e => this.mapper.Map<EntryDb>(e)).ToList();

            this.databaseRepository.AddEntries(entriesDb);

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

                    if(page < 1)
                    {
                        return new BadRequestObjectResult("PageId cannot be less than 1");
                    }

                    return new OkObjectResult(this.databaseRepository.GetEntries(page, limit));
                } catch (Exception)
                {
                    return new BadRequestObjectResult("PageId or pageLimit has incorrect form");
                }
                
            }

            if (pageLimit != null && pageId == null)
            {
                return new BadRequestObjectResult("PageId is needed when pageLimit is given.");
            }

            if (pageId != null && pageLimit == null)
            {
                return new BadRequestObjectResult("PageLimit is needed when PageId is given.");
            }

            return new OkObjectResult(this.databaseRepository.GetEntries());
        }

        [HttpPut]
        [Route("entries/{id}")]
        [Authorize(Policy = "Admin")]
        public IActionResult UpdateEntry(int id, Entry entry)
        {
            var entryToUpdate = this.mapper.Map<EntryDb>(entry);
            var entryUpdated = this.databaseRepository.UpdateEntry(id, entryToUpdate);

            if (entryToUpdate == null)
            {
                return new BadRequestObjectResult("Bad request");
            }

            return new OkObjectResult(entryUpdated);
        }
    }
}
