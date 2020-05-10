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
                return GetElementWithClassContent(HtmlElement.Span, "priceInfo__value").InnerText
                    .Contains("miesiąc")
                    ? OfferKind.RENTAL
                    : OfferKind.SALE;
            }
        }
 
        private string Email
        {
            get
            {
                if (RawDescription == null)
                {
                    return "N/A";
                }

                var emailPattern = "[A-Z0-9._-]+@[A-Z._-]+";
                var regex = new Regex(emailPattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
                var match = regex.Match(RawDescription);

                return match.Success ? match.Value : "N/A";
            }
        }

        private string Telephone
        {
            get
            {
                var telephone = GetElementWithId(HtmlElement.A, "pokaz-numer-dol")?
                    .Attributes[HtmlAttribute.DataFullPhoneNumber]
                    .Value;

                return telephone ?? "N/A";
            }
        }

        private string Name
        {
            get
            {
                return GetElementWithClassContent(HtmlElement.H3, "offerOwner__person")?.InnerText
                    ?? GetElementWithClassContent(HtmlElement.H3, "offerOwner__company")?.InnerText
                    ?? "N/A";
            }
        }



        public OfferDetailsParser(HtmlDocument htmlDocument) : base(htmlDocument)
        {
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
