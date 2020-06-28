using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IntegrationApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace IntegrationApi.Controllers
{
    [Route("oferty")]
    public class NajlepszeOfertyController : Controller
    {

        [HttpGet]
        [Route("NajlepszeOferty")]
        public IActionResult NajlepszeOferty()
        {
            // Najtańsze ogłoszenia z ceną za metr w danym mieście
            return Ok(new Okazje().ZwrocNajlepszeOferty(ObslugaBazyDanych.ZwrocRekordy()));
        }

    }
}