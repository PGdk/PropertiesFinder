using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntegrationApi.Models
{
    public class PageRequest
    {
        [JsonProperty("pageNumber")]
        public int PageNumber { get; set; }
    }
}
