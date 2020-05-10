using Models;
using Models.Trovit;
using System;

namespace Application.Trovit
{
    public class Filter
    {
        public TrovitPlaceKind Place { get; set; }
        public OfferKind Kind { get; set; }
    }

    public class TrovitCursor
    {
        private const string BASE_URL = "https://mieszkania.trovit.pl";

        public Filter filter;


        private int page = 1;

        public TrovitCursor(Filter filter) {
            this.filter = filter;
        }

        public string URL()
        {
            return $"{BASE_URL}/index.php/cod.search_homes/type.{kind(filter)}/what_d.{place(filter)}/page.{page}";
        }

        public void Next()
        {
            page++;
        }

        public int Page() {
            return page;
        }

        private int kind(Filter filter)
        {
            switch (filter.Kind)
            {
                case OfferKind.SALE:
                    return 1;
                case OfferKind.RENTAL:
                    return 2;
                default:
                    throw new NotImplementedException();
            }
        }

        private string place(Filter filter)
        {
            switch (filter.Place)
            {
                case TrovitPlaceKind.HOUSE:
                    return "dom";
                case TrovitPlaceKind.APARTMENT:
                    return "mieszkanie";
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
