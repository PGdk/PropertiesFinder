﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatabaseConnection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;

namespace IntegrationApi.Controllers
{
    [Route("[controller]")]
    [Authorize]
    [ApiController]
    public class EntriesController : ControllerBase
    {
        private IDatabaseContext DatabaseContext { get; set; }

        public EntriesController(IDatabaseContext databaseContext)
        {
            DatabaseContext = databaseContext;
        }

        [HttpGet]
        [Authorize(Policy = "User")]
        public IActionResult GetEntries([FromQuery(Name = "PageId")] int? pageId, [FromQuery(Name = "PageLimit")] int? pageLimit)
        {
            var query = DatabaseContext.Entries
                .Include(e => e.OfferDetails)
                    .ThenInclude(od => od.SellerContact)
                .Include(e => e.PropertyAddress)
                .Include(e => e.PropertyDetails)
                .Include(e => e.PropertyPrice)
                .Include(e => e.PropertyFeatures);

            if (pageId == null && pageLimit == null)
            {
                return Ok(query.ToList());
            }

            if (pageId != null && pageLimit != null)
            {
                return Ok(query
                    .Skip(((int)pageId - 1) * (int)pageLimit)
                    .Take((int)pageLimit)
                    .ToList());
            }

            return BadRequest("Both PageId and PageLimit must be given");
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "User")]
        public IActionResult GetEntry(int id)
        {
            var entry = DatabaseContext.Entries
                .Include(e => e.OfferDetails)
                    .ThenInclude(od => od.SellerContact)
                .Include(e => e.PropertyAddress)
                .Include(e => e.PropertyDetails)
                .Include(e => e.PropertyPrice)
                .Include(e => e.PropertyFeatures)
                .Where(e => e.ID == id)
                .FirstOrDefault();

            if (entry == null)
            {
                return NotFound($"No entry with the given id: {id}");
            }

            return Ok(entry);
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "Admin")]
        public IActionResult PutEntry(int id, [FromBody] Entry entry)
        {
            DatabaseContext.Entries.Update(entry);
            DatabaseContext.SaveChanges();

            return Ok();
        }

        /**
         * Returns an offers that are at least 25% cheaper than the average price per meter in the given city.
         */
        [HttpGet("/Deal")]
        [Authorize(Policy = "User")]
        public List<Entry> GetDeal(PolishCity city)
        {
            var entries = DatabaseContext.Entries
                .Where(e => e.PropertyAddress.City == city)
                .ToList();

            if (entries.Count == 0)
            {
                return new List<Entry>();
            }

            var maxPricePerMeter = 0.75m * entries.Average(e => e.PropertyPrice.PricePerMeter);

            var deals = entries
                .Where(e => e.PropertyPrice.PricePerMeter < maxPricePerMeter)
                .ToList();

            return deals;
        }
    }
}