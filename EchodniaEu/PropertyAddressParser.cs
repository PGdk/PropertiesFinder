using Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace EchodniaEu
{
    class PropertyAddressParser : OfferParser<PropertyAddress>
    {
        private string Aa
        {
            get
            {
                return GetFieldValue("Lokalizacja");
            }
        }
        public override PropertyAddress Dump()
        {
            //Console.WriteLine(Aa);
            var a = Aa;
            return new PropertyAddress();
        }
    }
}
