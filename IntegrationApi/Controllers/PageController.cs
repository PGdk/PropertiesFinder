using System.Threading.Tasks;
using DatabaseConnection;
using Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZnajdzTo;

namespace IntegrationApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PageController : ControllerBase
    {
        private readonly ZnajdzToIntegration _znajdzToIntegration;

        public PageController(IWebSiteIntegration znajdzToIntegration)
        {
            _znajdzToIntegration = (ZnajdzToIntegration)znajdzToIntegration;
        }

        [HttpPost]
        [Authorize]
        [Authorize(Policy = "User")]
        public async Task<ActionResult> LoadPage(Page page)
        {
            var entries = _znajdzToIntegration.TakeHomeSalesEntriesFromHomeSalesListPage(page.PageNumber);
            if (entries.Count == 0)
            {
                return NotFound();
            }

            using DatabaseContext databaseContext = new DatabaseContext();
            databaseContext.Entries.AddRange(entries);
            await databaseContext.SaveChangesAsync();

            return NoContent();
        }
    }
}
