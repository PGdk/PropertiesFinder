using Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Text.Json;

namespace SprzedawaczIntegration
{
    public class Comparer : IEqualityComparer<Entry>
    {
        public bool Equals(Entry x, Entry y)
        {
            return Equals(x, y, 1.0);
        }

        public bool Equals(Entry x, Entry y, double similarityLevel)
        {
            if (x.PropertyAddress.City != y.PropertyAddress.City)
                return false;
            if (x.OfferDetails.OfferKind != y.OfferDetails.OfferKind)
                return false;
            if (Math.Abs(x.PropertyDetails.Area - y.PropertyDetails.Area) > 1)
                return false;
            if (x.PropertyDetails.NumberOfRooms != y.PropertyDetails.NumberOfRooms)
                return false;

            var Scorer = new PropertiesScorer<Entry>(x, y);

            Scorer.ScoreProperties((e) => e.PropertyAddress.District);
            Scorer.ScoreProperties((e) => e.PropertyAddress.StreetName);
            Scorer.ScoreProperties((e) => e.PropertyAddress.DetailedAddress);
            Scorer.ScoreProperties((e) => e.OfferDetails.SellerContact);
            Scorer.ScoreProperties((e) => e.PropertyDetails.FloorNumber);
            Scorer.ScoreProperties((e) => e.PropertyDetails.YearOfConstruction);
            Scorer.ScoreProperties((e) => e.PropertyFeatures.Balconies);
            Scorer.ScoreProperties((e) => e.PropertyFeatures.BasementArea);
            Scorer.ScoreProperties((e) => e.PropertyFeatures.GardenArea);
            Scorer.ScoreProperties((e) => e.PropertyFeatures.IndoorParkingPlaces);
            Scorer.ScoreProperties((e) => e.PropertyFeatures.OutdoorParkingPlaces);
            if (Scorer.Score < similarityLevel)
                return false;

            return true;
        }

        private class PropertiesScorer<T>
        {
            private readonly T _x;
            private readonly T _y;
            private int _counter;
            private int _truths;
            public double Score => _truths / _counter;

            public PropertiesScorer(T x, T y)
            {
                _x = x;
                _y = y;
                _truths = 0;
                _counter = 0;
            }
            private bool CompareProperties<U>(Func<T, U> selector)
            {
                return selector(_x).Equals(selector(_y));
            }
            public void ScoreProperties<U>(Func<T, U> selector)
            {
                _counter++;
                if (CompareProperties(selector))
                    _truths++;
            }
        }

        public int GetHashCode([DisallowNull] Entry obj)
        {
            var hashBuilder  = new HashBuilder();

            hashBuilder.AddProperty(obj.PropertyAddress.City);
            hashBuilder.AddProperty(obj.OfferDetails.OfferKind);
            hashBuilder.AddProperty(obj.PropertyDetails.Area);
            hashBuilder.AddProperty(obj.PropertyDetails.NumberOfRooms);
            hashBuilder.AddProperty(obj.PropertyAddress.District);
            hashBuilder.AddProperty(obj.PropertyAddress.StreetName);
            hashBuilder.AddProperty(obj.PropertyAddress.DetailedAddress);
            hashBuilder.AddProperty(obj.OfferDetails.SellerContact);
            hashBuilder.AddProperty(obj.PropertyDetails.FloorNumber);
            hashBuilder.AddProperty(obj.PropertyDetails.YearOfConstruction);
            hashBuilder.AddProperty(obj.PropertyFeatures.Balconies);
            hashBuilder.AddProperty(obj.PropertyFeatures.BasementArea);
            hashBuilder.AddProperty(obj.PropertyFeatures.GardenArea);
            hashBuilder.AddProperty(obj.PropertyFeatures.IndoorParkingPlaces);
            hashBuilder.AddProperty(obj.PropertyFeatures.OutdoorParkingPlaces);

            return hashBuilder.Hash;
        }

        private class HashBuilder
        {
            private string _toHash;
            public int Hash => _toHash.GetHashCode();
            public HashBuilder()
            {
                _toHash = "";
            }

            public void AddProperty(object property)
            {
                _toHash = $"{_toHash}{JsonSerializer.Serialize(property)}";
            }
        }
    }
}
