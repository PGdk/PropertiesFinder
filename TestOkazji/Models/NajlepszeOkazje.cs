using Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PropertiesFinderTests.Models
{
    public static class NajlepszeOkazje
    {
        public static List<Entry> NajnizszaCenaZPojedynczegoMiasta()
        {
            return new List<Entry>
            {
                new Entry
                {
                    PropertyPrice = new PropertyPrice
                    {
                        PricePerMeter = 100000,
                    },
                    PropertyAddress = new PropertyAddress
                    {
                        City = PolishCity.GDANSK

                    }
                },
                new Entry
                {
                    PropertyPrice = new PropertyPrice
                    {
                        PricePerMeter = 0,
                    },
                    PropertyAddress = new PropertyAddress
                    {
                        City = PolishCity.GDANSK

                    }
                },
                new Entry
                {
                    PropertyPrice = new PropertyPrice
                    {
                        PricePerMeter = 50000,
                    },
                    PropertyAddress = new PropertyAddress
                    {
                        City = PolishCity.GDANSK

                    }
                },
                new Entry
                {
                    PropertyPrice = new PropertyPrice
                    {
                        PricePerMeter = 9999999,
                    },
                    PropertyAddress = new PropertyAddress
                    {
                        City = PolishCity.GDANSK

                    }
                },
                new Entry
                {
                    PropertyPrice = new PropertyPrice
                    {
                        PricePerMeter = 0,
                    },
                    PropertyAddress = new PropertyAddress
                    {
                        City = PolishCity.GDANSK

                    }
                },

            };
        }

        public static List<Entry> BrakWynikowDlaCenZaMetrRownych0()
        {
            return new List<Entry>
            {
                new Entry
                {
                    PropertyPrice = new PropertyPrice
                    {
                        PricePerMeter = 0,
                    },
                    PropertyAddress = new PropertyAddress
                    {
                        City = PolishCity.GDANSK

                    }
                },
                new Entry
                {
                    PropertyPrice = new PropertyPrice
                    {
                        PricePerMeter = 0,
                    },
                    PropertyAddress = new PropertyAddress
                    {
                        City = PolishCity.SOPOT

                    }
                },
                new Entry
                {
                    PropertyPrice = new PropertyPrice
                    {
                        PricePerMeter = 0,
                    },
                    PropertyAddress = new PropertyAddress
                    {
                        City = PolishCity.GDYNIA

                    }
                },
                new Entry
                {
                    PropertyPrice = new PropertyPrice
                    {
                        PricePerMeter = 0,
                    },
                    PropertyAddress = new PropertyAddress
                    {
                        City = PolishCity.WEJHEROWO

                    }
                },
                new Entry
                {
                    PropertyPrice = new PropertyPrice
                    {
                        PricePerMeter = 0,
                    },
                    PropertyAddress = new PropertyAddress
                    {
                        City = PolishCity.RUMIA

                    }
                },

            };
        }

        public static List<Entry> JedenWynikNaJednoMiasto()
        {
            return new List<Entry>
            {
                new Entry
                {
                    PropertyPrice = new PropertyPrice
                    {
                        PricePerMeter = 100000,
                    },
                    PropertyAddress = new PropertyAddress
                    {
                        City = PolishCity.GDANSK

                    }
                },
                new Entry
                {
                    PropertyPrice = new PropertyPrice
                    {
                        PricePerMeter = 30023,
                    },
                    PropertyAddress = new PropertyAddress
                    {
                        City = PolishCity.GDANSK

                    }
                },
                new Entry
                {
                    PropertyPrice = new PropertyPrice
                    {
                        PricePerMeter = 5670,
                    },
                    PropertyAddress = new PropertyAddress
                    {
                        City = PolishCity.GDYNIA

                    }
                },
                new Entry
                {
                    PropertyPrice = new PropertyPrice
                    {
                        PricePerMeter = 9840,
                    },
                    PropertyAddress = new PropertyAddress
                    {
                        City = PolishCity.GDYNIA

                    }
                },
                new Entry
                {
                    PropertyPrice = new PropertyPrice
                    {
                        PricePerMeter = 4590,
                    },
                    PropertyAddress = new PropertyAddress
                    {
                        City = PolishCity.RUMIA

                    }
                },
                new Entry
                {
                    PropertyPrice = new PropertyPrice
                    {
                        PricePerMeter = 7860,
                    },
                    PropertyAddress = new PropertyAddress
                    {
                        City = PolishCity.RUMIA

                    }
                },

            };
        }
    }
}
