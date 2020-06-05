using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatabaseConnection;
using IntegrationApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IntegrationApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class pageController : ControllerBase
    {
        [HttpPost]
        [Authorize]
        public IActionResult AddPage([FromBody] PageSelect pageSelect)
        {
            BezposrednieIntegrationRepo repo = new BezposrednieIntegrationRepo();
            if (Request.Headers.ContainsKey("X-Request-ID"))
            {
                if (Request.Headers["X-Request-ID"].ToString().Trim() != "")
                {
                    repo.AddLog(Request.Headers["X-Request-ID"]);
                }
            }
            var Entries = repo.Split(pageSelect.pageNumber);
            if (Entries.Count() != 0)
            {
                repo.AddEntry(Entries);
                return Ok(Entries);
            }
            else
            {
                return BadRequest("No Page");
            }
        }
    }
}