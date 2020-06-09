using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatabaseConnection;
using Application;
using IntegrationApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace IntegrationApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PageController : ControllerBase
    {
        private DatabaseContext _context { get; set; }

        public PageController(DatabaseContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult SavePage([FromBody] Page pageId)
        {
            try
            {
                var dump = new Lento().GenerateDumpFromPage(pageId.PageNumber);
                foreach(var entry in dump.Entries)
                {
                    _context.Add(entry);
                }
                _context.SaveChanges();

                return NoContent();
            }
            catch 
            {
                return NotFound($"Page {pageId.PageNumber} doesn't exist");
            }


        }
    }
}