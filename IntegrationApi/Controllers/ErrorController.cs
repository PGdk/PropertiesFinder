﻿using Microsoft.AspNetCore.Mvc;

namespace IntegrationApi.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error/{statusCode}")]
        public IActionResult ObslugaBledow(int statusCode)
        {
            switch(statusCode)
            {
                case 404:
                    return NotFound("Nie znaleziono strony");
                    break;
            }
            return View();
        }
    }
}