using System;
using Models;

namespace UnitTests
{
    public class EntryTemplateAdapter: Entry
    {
        public string OfferDetailsOfferKindText =>
            OfferDetails.OfferKind switch
            {
                OfferKind.RENTAL => "wynajęcia",
                OfferKind.SALE => "sprzedaż",
                _ => string.Empty
            };

        public string OfferDetailsCreationDateTime => OfferDetails.CreationDateTime.ToString("dd-MM-yyyy");
        public string OfferDetailsLastUpdateDateTime => OfferDetails.LastUpdateDateTime.Value.ToString("dd-MM-yyyy");

    }
}