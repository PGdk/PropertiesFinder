using HtmlAgilityPack;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace EchodniaEu
{
    class OfferDetailsParser : OfferParser<OfferDetails>
    {
        public string Url { get; set; }

        public HtmlNode OfferHeader { get; set; }

        private DateTime CreationDateTime {
            get {
                return DateTime.Parse(OfferHeader
                    .Descendants(HtmlElement.Time)
                    .First()
                    .InnerText
                );
            }
        }

        private DateTime? LastUpdateDateTime
        {
            get
            {
                var lastUpdateDateTime = OfferHeader
                    .Descendants(HtmlElement.Time)
                    .ElementAtOrDefault(1)?
                    .InnerText;

                if (lastUpdateDateTime != null)
                {
                    return DateTime.Parse(lastUpdateDateTime);
                }
                return null;
            }
        }

        private OfferKind OfferKind
        {
            get
            {
                return GetElementWithClassContent(HtmlElement.Span, "priceInfo__value")
                    .Contains("miesiąc")
                    ? OfferKind.RENTAL
                    : OfferKind.SALE;
            }
        }
 
        private string Email
        {
            get
            {
                var emailMatch = "[A-Z0-9._-]+@[A-Z._-]+";
                var regex = new Regex(emailMatch, RegexOptions.Compiled | RegexOptions.IgnoreCase);
                var match = regex.Match(RawDescription);

                return match.Success ? match.Value : "N/A";
            }
        }

        private string Telephone
        {
            get
            {
                var telephone = HtmlDocument.DocumentNode
                    .Descendants(HtmlElement.A)
                    .Where(a => a.Id == "pokaz-numer-dol")
                    .FirstOrDefault()?.Attributes["data-full-phone-number"].Value;

                return telephone != null ? telephone : "N/A";
            }
        }

        private string Name
        {
            get
            {
                var name = HtmlDocument.DocumentNode
                    .Descendants(HtmlElement.H3)
                    .Where(strong => strong.HasClass("offerOwner__person"))
                    .FirstOrDefault()?.InnerText;

                if (name == null)
                {
                    name = HtmlDocument.DocumentNode
                    .Descendants(HtmlElement.H3)
                    .Where(strong => strong.HasClass("offerOwner__company"))
                    .FirstOrDefault()?.InnerText;
                }

                return name != null ? name : "N/A";
            }
        }

        public override OfferDetails Dump()
        {
            return new OfferDetails
            {
                Url = Url,
                CreationDateTime = CreationDateTime,
                LastUpdateDateTime = LastUpdateDateTime,
                OfferKind = OfferKind,
                SellerContact = new SellerContact
                {
                    Email = Email,
                    Telephone = Telephone,
                    Name = Name
                },
                IsStillValid = true,
            };
        }
    }
}
