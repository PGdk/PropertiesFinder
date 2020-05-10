using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;

namespace EchodniaEu
{
    class OffersListPage: Page
    {
        private static string CategoriesSectionId = "ogloszenia-kategorie";
        private static string OffersSectionId = "lista-ogloszen";
        private static string PropertyLinkText = "Nieruchomości";
        private static string NextPageDataGtmAttribute = "nowa_karta/nawigator/nastepna";

        public OffersListPage(string url) : base(url)
        {
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
    }

    
}
