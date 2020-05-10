using Application.Trovit.Parser;
using Interfaces.Trovit;
using Models;

namespace Application.Trovit.Enhance
{
    internal class TabelaOfertEnhancer : ITrovitEnhancer
    {

        private TabelaOfertDetails details;

        public TabelaOfertEnhancer(TabelaOfertDetails details) {
            this.details = details;
        }
        
        public Entry Enhance(Entry entry)
        {

            entry.OfferDetails = details.OfferDetails;
            entry.PropertyAddress = details.PropertyAddress;
            entry.PropertyPrice = details.PropertyPrice;
            entry.PropertyDetails = details.PropertyDetails;

            return entry;
        }

    }
}
