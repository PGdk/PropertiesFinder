using Models;

namespace Exhouse.Exhouse
{
    class ExhouseWebPage : WebPage
    {
        public const string OFFERS_PATH = "oferty";

        private const string URL = "https://www.exhouse.pl/";
        private const string NAME = "Exhouse.pl - nieruchomości";

        public ExhouseWebPage()
        {
            Url = URL;
            Name = NAME;
            WebPageFeatures = new WebPageFeatures
            {
                HomeSale = true,
                HomeRental = true,
                HouseSale = true,
                HouseRental = true
            };
        }
    }
}
