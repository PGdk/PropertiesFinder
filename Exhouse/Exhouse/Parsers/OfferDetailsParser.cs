using System;
using HtmlAgilityPack;
using Models;

namespace Exhouse.Exhouse.Parsers
{
    public class OfferDetailsParser
    {
        public static OfferDetails Parse(HtmlNode documentNode)
        {
            HtmlNode headerInfoNode = documentNode.SelectSingleNode("//div[@class='col-md-8 offer-head-top']/h2[@class='small']/text()");
            HtmlNode telephoneNode = documentNode.SelectSingleNode("//a[@class='agent-mobile']/text()");
            HtmlNode nameNode = documentNode.SelectSingleNode("//div[@class='agent-name']/text()");

            OfferDetails offerDetails = new OfferDetails
            {
                CreationDateTime = DateTime.Now,
                OfferKind = headerInfoNode.InnerText.Contains("sprzedaż") ? OfferKind.SALE : OfferKind.RENTAL,
                SellerContact = new SellerContact(),
                IsStillValid = true
            };

            if (null != telephoneNode)
            {
                offerDetails.SellerContact.Telephone = telephoneNode.InnerText.Trim();
            }

            if (null != nameNode)
            {
                offerDetails.SellerContact.Name = nameNode.InnerText.Trim();
            }

            return offerDetails;
        }
    }
}
