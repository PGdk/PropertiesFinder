using HtmlAgilityPack;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EchodniaEu
{
    class OfferDetailsParser : PageParser<OfferDetails>
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
                return lastUpdateDateTime != null
                    ? DateTime.Parse(lastUpdateDateTime)
                    : new DateTime();
            }
        }

        private string Email
        {
            get
            {
                return "N/A";
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
                OfferKind = OfferKind.SALE,
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
