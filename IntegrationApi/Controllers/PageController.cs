using System.Threading.Tasks;
using DatabaseConnection;
using Exhouse.Exhouse;
using IntegrationApi.Models;
using Microsoft.AspNetCore.Mvc;

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
                return BadRequest();
            }

            await _context.Entries.AddRangeAsync(
                _integration.FetchEntriesFromOffersPage(selectedPage.PageNumber)
            );

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}