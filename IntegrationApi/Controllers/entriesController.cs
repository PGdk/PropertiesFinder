using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatabaseConnection;
using IntegrationApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace IntegrationApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class entriesController : ControllerBase
    {
        private BezposrednieIntegrationRepo repo = new BezposrednieIntegrationRepo();

        Info info = new Info()
        {
            connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=PropertiesFinder;Integrated Security=True",
            integrationName = "BezposrednieIntegration",
            studentName = "Sebastian Jedynak",
            studentIndex = "131525"
        };

        [HttpGet]
        [Route("info")]
        public IActionResult GetInfo()
        {
            if (Request.Headers.ContainsKey("X-Request-ID"))
            {
                if (Request.Headers["X-Request-ID"].ToString().Trim() != "")
                {
                    repo.AddLog(Request.Headers["X-Request-ID"]);
                }
            }
            return Ok(info);
        }

        [HttpGet("{id}")]
        [Authorize]
        public IActionResult GetEntry(string id)
        {
            if (Request.Headers.ContainsKey("X-Request-ID"))
            {
                if (Request.Headers["X-Request-ID"].ToString().Trim() != "")
                {
                    repo.AddLog(Request.Headers["X-Request-ID"]);
                }
            }
            var EntriesResult = GetEntries() as OkObjectResult;
            var EntriesResultToList = EntriesResult.Value as List<Entry>;
            if (EntriesResultToList.Count < int.Parse(id))
            {
                return BadRequest();
            }
            return Ok(EntriesResultToList[int.Parse(id) - 1]);
        }

        [HttpGet]
        [Authorize]
        public IActionResult GetEntries()
        {
            if (Request.Headers.ContainsKey("X-Request-ID"))
            {
                if (Request.Headers["X-Request-ID"].ToString().Trim() != "")
                {
                    repo.AddLog(Request.Headers["X-Request-ID"]);
                }
            }
            return Ok(repo.GetEntries());
        }
    }
}