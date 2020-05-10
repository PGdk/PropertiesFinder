using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;

namespace EchodniaEu
{
    class OffersListPage
    {
        private static string CategoriesSectionId = "ogloszenia-kategorie";
        private static string OffersSectionId = "lista-ogloszen";
        private static string PropertyLinkText = "Nieruchomości";
        private static string NextPageDataGtmAttribute = "nowa_karta/nawigator/nastepna";
        private static string LastPageDataGtmAttribute = "nowa_karta/nawigator/ostatnia";

        protected string Url { get; set; }

        protected HtmlDocument HtmlDocument { get; set; }

        public OffersListPage(string url)
        {
            Url = url;
            HtmlDocument = new HtmlWeb().Load(url);
        }

        internal IEnumerable<(string, HtmlNode)> GetOfferUrls()
        {
            return HtmlDocument.DocumentNode
                .Descendants(HtmlElement.Section)
                .Where(s => s.Attributes[HtmlAttribute.Id]?.Value == OffersSectionId)
                .First()
                .Descendants(HtmlElement.Ul)
                .First()
                .Descendants(HtmlElement.Li)
                .Select(li => li.Descendants(HtmlElement.A).First())
                .Select(a => (a.Attributes[HtmlAttribute.Href]?.Value, a))
                .ToList();
        }

        public OffersListPage GetPropertyOffersPage()
        {
            var propertyOffersUrl = HtmlDocument.DocumentNode
                .Descendants(HtmlElement.Section)
                .Where(s => s.Attributes[HtmlAttribute.Id]?.Value == CategoriesSectionId)
                .First()
                .Descendants(HtmlElement.A)
                .Where(a => a.InnerText.Contains(PropertyLinkText))
                .First()
                .Attributes[HtmlAttribute.Href]
                .Value;

            return new OffersListPage(propertyOffersUrl);
        }

        public OffersListPage GetNextOffersListPage()
        {
            var nextPageUri = HtmlDocument.DocumentNode
                .Descendants(HtmlElement.A)
                .Where(a => a.Attributes[HtmlAttribute.DataGtm]?.Value == NextPageDataGtmAttribute)
                .FirstOrDefault()?
                .Attributes[HtmlAttribute.Href]
                .Value;

            return nextPageUri != null 
                ? new OffersListPage(Url.Split("/ogloszenia").First() + nextPageUri)
                : null;
        }

        public int GetPagesCount()
        {
            var url = HtmlDocument.DocumentNode
                .Descendants(HtmlElement.A)
                .Where(a => a.Attributes[HtmlAttribute.DataGtm]?.Value == LastPageDataGtmAttribute)
                .FirstOrDefault()?
                .Attributes[HtmlAttribute.Href]
                .Value;

            return int.Parse(
                url.Split(',')
                    .First()
                    .Replace("/ogloszenia/", "")
            );
        }
    }

    
}
