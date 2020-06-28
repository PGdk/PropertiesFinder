using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Microsoft.AspNetCore.Authorization;
using DatabaseConnection.Interfaces;
using IntegrationApi.Interfaces;

namespace IntegrationApi.Controllers
{
    [ApiController]
    public class EntriesController : ControllerBase
    {
        private static readonly int OccasionalEntriesLimit = 5;

        private static readonly string PageLimitQueryParameterName = "pageLimit";
        private static readonly string PageIdQueryParameterName = "pageId";

        private readonly IEntriesRepository _repository;

        private readonly IOccasionalEntriesProvider _provider;

        public EntriesController(IEntriesRepository repository, IOccasionalEntriesProvider provider)
        {
            _repository = repository;
            _provider = provider;
        }

        // GET: /entries/occasional?city=TOURN
        [Route("entries/occasional")]
        [Authorize(Policy = "User")]
        [HttpGet]
        public async Task<ActionResult<List<Entry>>> GetOccasionalEntries(PolishCity city)
        {
            return await _provider.GetByCity(city, OccasionalEntriesLimit);
        }

        // GET: /entries || /entries?pageLimit=8&pageId=2
        [Route("entries")]
        [Authorize(Policy = "User")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Entry>>> GetEntries()
        {
            int? pageLimit = Request.Query.ContainsKey(PageLimitQueryParameterName)
                ? int.Parse(Request.Query[PageLimitQueryParameterName])
                : (int?) null;
            int? pageId = Request.Query.ContainsKey(PageIdQueryParameterName)
                ? int.Parse(Request.Query[PageIdQueryParameterName])
                : (int?) null;

            if ((null != pageLimit && null == pageId) || (null == pageLimit && null != pageId))
            {
                return BadRequest();
            }

            if (null != pageLimit && pageLimit < 1)
            {
                return BadRequest();
            }

            if (null != pageId && (pageId < 1 || (await _repository.CountAll() / pageLimit) < (pageId - 1)))
            {
                return NotFound();
            }

            return await _repository.FindAll(pageLimit * (pageId - 1), pageLimit);
        }

        // GET: /entry/5
        [Route("entry/{id:int}")]
        [Authorize(Policy = "User")]
        [HttpGet]
        public async Task<ActionResult<Entry>> GetEntry(int id)
        {
            Entry entry = await _repository.Find(id);

            if (null == entry)
            {
                return NotFound();
            }

            return entry;
        }

        // PUT: /entry/5
        [Route("entry/{id:int}")]
        [Authorize(Policy = "Admin")]
        [HttpPut]
        public async Task<IActionResult> PutEntry(int id, Entry entry)
        {
            if (id != entry.Id)
            {
                return BadRequest();
            }

            try
            {
                await _repository.Save(entry);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (_repository.Exists(id))
                {
                    throw;
                }

                return NotFound();
            }

            return NoContent();
        }
    }
}
