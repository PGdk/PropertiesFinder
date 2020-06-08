using System;
using System.Collections.Generic;
using System.Linq;
using IntegrationApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace IntegrationApi.Controllers
{
    [Route("entries")]
    public class EntriesController : Controller
    {
        [HttpGet]
        [Route("GetInfo")]
        public Dictionary<string, string> GetInfo()
        {
            return new Dictionary<string, string>
            {
                { "connectionString", "Data Source = DESKTOP - U7HDE2F\\SQLEXPRESS; Initial Catalog = Jan_M_170630; Integrated Security = True"},
                { "integrationName", "DziennikBałtycki"},
                { "studentName", "Jan" },
                { "studentIndex", "170630" }
            };
        }

        [HttpGet]
        [Route("GetEntries")]
        [Authorize(Policy = "User")]
        public IActionResult GetEntries(string pageLimit, string pageId)
        {
            if(pageLimit == null && pageId == null)
            {
                return RedirectToAction("GetAllEntries");
            }
            else if (pageLimit == null || pageId == null)
            {
                return BadRequest("Nieprawidłowa wartość");
            }
            else if(!pageLimit.All(x => char.IsDigit(x)) || !pageId.All(x => char.IsDigit(x)))
            {
                return BadRequest("Nieprawidłowa wartość");
            }
            else
            {
                return Ok(ObslugaBazyDanych.ZwrocRekordy(Convert.ToInt32(pageLimit), Convert.ToInt32(pageId)));
            }

        }
        [Route("GetAllEntries")]
        [Authorize(Policy = "User")]
        public IActionResult GetAllEntries()
        {
            return Ok(ObslugaBazyDanych.ZwrocRekordy());
        }

        [HttpGet]
        [Route("{id}")]
        [Authorize(Policy = "User")]
        public IActionResult GetEntry(string id)
        {

            if(!id.All(x => char.IsDigit(x)))
            {
                return BadRequest("Żadanie musi być liczbą");
            }
            else
            {
                return Ok(ObslugaBazyDanych.ZwrocRekord(Convert.ToInt32(id)));
            }
        }

        [HttpPut]
        [Route("UpdateEntry")]
        [Authorize(Policy = "Admin")]
        public IActionResult UpdateEntry(int? id,[FromBody]Entry Entry)
        {
            if(ModelState.IsValid)
            {
                if(id != null)
                {
                    Entry.ID = (int)id;
                    bool Sukces = ObslugaBazyDanych.AktualizujRekord(Entry);

                    if (Sukces == false)
                    {
                        return NotFound("Nie istnieje element o podanym Id");
                    }
                    return Ok("Pomyslnie zaktualizowany");
                }
                else
                {
                    return BadRequest("Potrzebny jest id elementu ktory ma zostac zmodyfikowany");
                }
            }
            else
            {
                return BadRequest(Entry);
            }

            

        }

    }
}