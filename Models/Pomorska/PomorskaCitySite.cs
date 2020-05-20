using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Pomorska
{
    public class PomorskaCitySite
    {
        public string URL { get; set; }
        public int SubPagesQuantity { get; set; }
        public List<GratkaSite> GratkaUrlAndDate { get; set; } = new List<GratkaSite>();
    }
}
