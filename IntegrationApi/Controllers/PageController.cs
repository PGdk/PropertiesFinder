using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatabaseConnection;
using EchodniaEu;
using IntegrationApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace IntegrationApi.Controllers
{
    [Route("[controller]")]
    [Authorize]
    [ApiController]
    public class PageController : ControllerBase
    {
        private DatabaseContext DatabaseContext { get; set; }

        public PageController(DatabaseContext databaseContext)
        {
            DatabaseContext = databaseContext;
        }

        [HttpPost]
        [Authorize(Policy = "User")]
        public IActionResult SavePage([FromBody] PageDto dto)
        {
            try
            {
                var dump = new EchodniaEuParser()
                {
                    WebPage = new WebPage
                    {
                        Url = "https://echodnia.eu/ogloszenia",
                        Name = "Echodnia.eu Integration",
                        WebPageFeatures = new WebPageFeatures
                        {
                            HomeSale = true,
                            HomeRental = false,
                            HouseSale = true,
                            HouseRental = false
                        }
                    }
                }.Parse(dto.PageNumber);

                dump.Entries.ToList().ForEach(e => DatabaseContext.Add(e));
                DatabaseContext.SaveChanges();

                return NoContent();
            }
            catch (NotFoundException e)
            {
                return NotFound($"Page {dto.PageNumber} doesn't exist");
            }


        }
    }
}