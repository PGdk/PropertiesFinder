using Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bazos
{
    public class MockEntriesLists
    {
        public static List<Entry> MakeASmallList()
        {
            List<Entry> entries = new List<Entry>();

            Entry entry1 = new Entry
            {
                OfferDetails = new OfferDetails
                {
                    OfferKind = OfferKind.RENTAL,
                },
                PropertyFeatures = new PropertyFeatures
                {
                    Balconies = 1,
                    OutdoorParkingPlaces = 1,
                    IndoorParkingPlaces = 0,
                },
            };

            Entry entry2 = new Entry
            {
                OfferDetails = new OfferDetails
                {
                    OfferKind = OfferKind.RENTAL,
                },
                PropertyFeatures = new PropertyFeatures
                {
                    Balconies = 1,
                    OutdoorParkingPlaces = 0,
                    IndoorParkingPlaces = 1,
                },
            };

            entries.Add(entry1);
            entries.Add(entry2);

            return entries;
        }

        public static List<Entry> MakeALongList()
        {
            List<Entry> entries = new List<Entry>();

            Entry entry1 = new Entry
            {
                OfferDetails = new OfferDetails
                {
                    OfferKind = OfferKind.RENTAL,
                },
                PropertyFeatures = new PropertyFeatures
                {
                    Balconies = 1,
                    OutdoorParkingPlaces = 1,
                    IndoorParkingPlaces = 0,
                },
            };

            Entry entry2 = new Entry
            {
                OfferDetails = new OfferDetails
                {
                    OfferKind = OfferKind.RENTAL,
                },
                PropertyFeatures = new PropertyFeatures
                {
                    Balconies = 1,
                    OutdoorParkingPlaces = 0,
                    IndoorParkingPlaces = 1,
                },
            };

            Entry entry3 = new Entry
            {
                OfferDetails = new OfferDetails
                {
                    OfferKind = OfferKind.RENTAL,
                },
                PropertyFeatures = new PropertyFeatures
                {
                    Balconies = 1,
                    OutdoorParkingPlaces = 1,
                    IndoorParkingPlaces = 0,
                },
            };

            Entry entry4 = new Entry
            {
                OfferDetails = new OfferDetails
                {
                    OfferKind = OfferKind.RENTAL,
                },
                PropertyFeatures = new PropertyFeatures
                {
                    Balconies = 1,
                    OutdoorParkingPlaces = 0,
                    IndoorParkingPlaces = 1,
                },
            };

            Entry entry5 = new Entry
            {
                OfferDetails = new OfferDetails
                {
                    OfferKind = OfferKind.RENTAL,
                },
                PropertyFeatures = new PropertyFeatures
                {
                    Balconies = 1,
                    OutdoorParkingPlaces = 1,
                    IndoorParkingPlaces = 0,
                },
            };

            Entry entry6 = new Entry
            {
                OfferDetails = new OfferDetails
                {
                    OfferKind = OfferKind.RENTAL,
                },
                PropertyFeatures = new PropertyFeatures
                {
                    Balconies = 1,
                    OutdoorParkingPlaces = 0,
                    IndoorParkingPlaces = 1,
                },
            };

            entries.Add(entry1);
            entries.Add(entry2);
            entries.Add(entry3);
            entries.Add(entry4);
            entries.Add(entry5);
            entries.Add(entry6);

            return entries;
        }

        public static List<Entry> MakeABadList()
        {
            List<Entry> entries = new List<Entry>();

            Entry entry1 = new Entry
            {
                OfferDetails = new OfferDetails
                {
                    OfferKind = OfferKind.RENTAL,
                },
                PropertyFeatures = new PropertyFeatures
                {
                    Balconies = 0,
                    OutdoorParkingPlaces = 1,
                    IndoorParkingPlaces = 0,
                },
            };

            Entry entry2 = new Entry
            {
                OfferDetails = new OfferDetails
                {
                    OfferKind = OfferKind.RENTAL,
                },
                PropertyFeatures = new PropertyFeatures
                {
                    Balconies = 1,
                    OutdoorParkingPlaces = 0,
                    IndoorParkingPlaces = 0,
                },
            };

            Entry entry3 = new Entry
            {
                OfferDetails = new OfferDetails
                {
                    OfferKind = OfferKind.SALE,
                },
                PropertyFeatures = new PropertyFeatures
                {
                    Balconies = 1,
                    OutdoorParkingPlaces = 1,
                    IndoorParkingPlaces = 1,
                },
            };

            entries.Add(entry1);
            entries.Add(entry2);
            entries.Add(entry3);

            return entries;
        }
    }
}
