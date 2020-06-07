using System;
using System.Collections.Generic;
using DatabaseConnection.Models;
using IntegrationApi.Models;
using IntegrationApi.Services;
using Microsoft.AspNetCore.Mvc;
using MEntry = Models.Entry;

namespace IntegrationApi.Controllers
{
    [ApiController]
    public class EntriesController : ControllerBase
    {
        private readonly IEntriesService entriesService;

        public EntriesController(IEntriesService entriesService)
        {
            this.entriesService = entriesService ?? throw new ArgumentException("Argument cannot be null", nameof(entriesService));
        }

        [HttpPost("page")]
        public IActionResult AddPage(PageRequest page)
        {
            if (page.PageNumber < 1)
                return new BadRequestObjectResult("Page number cannot be less than 1");

            try
            {
                this.entriesService.AddEntriesFromPage(page.PageNumber);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }

            return new NoContentResult();
        }

        [HttpGet("entries")]
        public IActionResult GetEntries([FromQuery]string pageLimit, [FromQuery]string pageId)
        {
            if((pageLimit == null && pageId != null) || (pageLimit != null && pageId == null))
            {
                return BadRequest("You have to provide both pageLimit and pageId or nither.");
            }
            else if(pageLimit != null && pageId != null)
            {
                int pageLimitNum, pageIdNum;
                if(!int.TryParse(pageLimit, out pageLimitNum) || !int.TryParse(pageId, out pageIdNum))
                {
                    return BadRequest("pageId or pageLimit has incorrect format.");
                }

                return Ok(this.entriesService.GetEntries(pageIdNum, pageLimitNum));
            }
            else
            {
                return Ok(this.entriesService.GetEntries());
            }
        }

        [HttpGet("entry/{id}")]
        public IActionResult GetEntry(long id)
        {
            var result = this.entriesService.GetEntry(id);
            if (result == null)
            {
                return NotFound();
            }
            else
            {
                return Ok();
            }
        }

        [HttpPut("entry/{id}")]
        public IActionResult UpdateEntry(long id, [FromBody]MEntry entry)
        {
            if(entry == null)
            {
                return BadRequest("Entry cannot be null");
            }

            try
            {
                this.entriesService.UpdateEntry(id, entry);
                return Ok();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
