using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using DatabaseConnection;
using Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace IntegrationApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize(Roles = "User,Admin")]
    public class PageController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebSiteIntegration _siteIntegration;

        public PageController(ApplicationDbContext context,  IWebSiteIntegration siteIntegration)
        {
            _context = context;
            _siteIntegration = siteIntegration;
        }

        [HttpPost]
        public async Task<ActionResult<IEnumerable<Entry>>> PostEntry(Page page)
        {
            var dump = _siteIntegration
                .DumpsRepository
                .GetAllDumpDetails(_siteIntegration.WebPage)
                .FirstOrDefault();

            var dumpEntries = _siteIntegration
                .DumpsRepository
                .GetDump(dump)
                .Entries;

            if (dumpEntries.Count() < page.PageSize * page.PageNumber)
            {
                return NotFound();
            }

            var entries = dumpEntries
                .Skip((page.PageNumber - 1) * page.PageSize)
                .Take(page.PageSize)
                .ToList();
            
            _context.Entries.AddRange(entries);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEntities", "Entries", null,  entries);
        }

        public class Page
        {
            [Required]
            [Range(1, int.MaxValue)]
            public int PageNumber { get; set; }
            
            [Range(1, int.MaxValue)]
            [DefaultValue(25)]
            public int PageSize { get; set; } = 25;
        }
    }
}