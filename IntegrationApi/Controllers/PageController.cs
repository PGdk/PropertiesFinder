using System.Collections.Generic;
using System.Threading.Tasks;
using DatabaseConnection;
using Exhouse.Exhouse;
using IntegrationApi.Models;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace IntegrationApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PageController : ControllerBase
    {
        private readonly ExhouseIntegration _integration;
        private readonly DatabaseContext _context;

        public PageController(DatabaseContext context)
        {
            _integration = ExhouseIntegrationFactory.Create();
            _context = context;
        }

        // POST: /page
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] SelectedPage selectedPage)
        {
            if (selectedPage.PageNumber < 1)
            {
                return NotFound();
            }

            List<Entry> entries = _integration.FetchEntriesFromOffersPage(selectedPage.PageNumber);

            if (0 == entries.Count)
            {
                return NotFound();
            }

            await _context.Entries.AddRangeAsync(entries);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}