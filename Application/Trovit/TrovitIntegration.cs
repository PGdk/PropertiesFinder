using Interfaces;
using Models;
using Models.Trovit;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.Trovit
{
    public class TrovitIntegration : IWebSiteIntegration
    {
        public WebPage WebPage { get; } = new WebPage
        {
            Url = "https://mieszkania.trovit.pl",
            Name = "mieszkania.trovit.pl",
            WebPageFeatures = new WebPageFeatures
            {
                HomeSale = true,
                HomeRental = true,
                HouseSale = true,
                HouseRental = true
            }
        };

        public IDumpsRepository DumpsRepository { get; }

        public IEqualityComparer<Entry> EntriesComparer { get; }

        private static Filter[] filters = {
            new Filter {
                Place = TrovitPlaceKind.APARTMENT,
                Kind = OfferKind.SALE,
            },
            new Filter {
                Place = TrovitPlaceKind.APARTMENT,
                Kind = OfferKind.RENTAL,
            },
            new Filter {
                Place = TrovitPlaceKind.HOUSE,
                Kind = OfferKind.SALE,
            },
            new Filter {
                Place = TrovitPlaceKind.HOUSE,
                Kind = OfferKind.RENTAL,
            },
        };

        public TrovitIntegration(IDumpsRepository dumpsRepository, IEqualityComparer<Entry> equalityComparer)
        {
            DumpsRepository = dumpsRepository;
            EntriesComparer = equalityComparer;
        }

        public Dump GenerateDump()
        {
            var entries = new TorvitClient().Fetch(filters[1], 1).Select(entry => {
                var extension = TrovitExtensionFactory.New(entry);

                if (extension == null)
                    return entry;

                var enhancer = extension.Handle();
                if (enhancer == null)
                    return entry;

                return enhancer.Enhance(entry);
            });

            return new Dump
            {
                DateTime = DateTime.Now,
                WebPage = WebPage,
                Entries = entries,
            };
        }
    }
}
