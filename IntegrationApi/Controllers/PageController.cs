using IntegrationApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IntegrationApi.Controllers
{
    [Route("page")]
    public class PageController : Controller
    {
        [HttpPost]
        [Route("LoadPage")]
        [Authorize(Policy = "User")]
        public  IActionResult page([FromBody] NumerStrony pageNumber)
        {
            if (pageNumber==null)
            {
                return BadRequest("Nieprawidłowa wartość");
            }
            else
            {
                 var oferty = ObslugaBazyDanych.ZwrocWybraneOferty(ObslugaBazyDanych.ZwrocListeOfert(ObslugaBazyDanych.ZwrocScierzke()), pageNumber.pageNumber);
                 ObslugaBazyDanych.WprowadzDOBazy(ref oferty);
                 return Ok(oferty);
            }
        }
    }
}