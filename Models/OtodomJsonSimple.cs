using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace Models
{
   public class OtodomJsonSimple
   {
        public Initialprops initialProps { get; set; }

        public class Initialprops
        {
            public Meta meta { get; set; }
            public Data data { get; set; }

        }
        public class Meta
        {
            public Target target { get; set; }
        }

        public class Target
        {
            public int Price { get; set; }
            public string[] Floor_no { get; set; }
            public string[] Rooms_num { get; set; }
            public string Area { get; set; }
            public int Price_per_m { get; set; }
            public string City { get; set; }
            public string[] Heating { get; set; }
            public string Build_year { get; set; }
            public string Building_floors_num { get; set; }
            public string OfferType { get; set; }
        }

        public class Data
        {
            public Advert advert { get; set; }
        }
        public class Advert
        {
            public Characteristic[] characteristics { get; set; }
            public string status { get; set; }
            public string url { get; set; }
            public string dateCreated { get; set; }
            public string dateModified { get; set; }
        }
        public class Characteristic
        {
            public string key { get; set; }
            public string currency { get; set; }
            public string label { get; set; }
            public string value { get; set; }
            public string value_translated { get; set; }
        }
    }
}
