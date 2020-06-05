using Models;

namespace Exhouse.Exhouse
{
    public class ExhouseWebPage : WebPage
    {
        public const string OFFERS_PATH = "oferty";
        public const string OFFERS_PAGE_PATH = "oferty/?page=";

        public const string URL = "https://www.exhouse.pl/";
        public const string NAME = "Exhouse.pl - nieruchomości";

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
