using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
//Klasa pozwalająca na podstawie JSONa ze strony otodom stworzyć obiekt Entry
namespace Models
{
    public class OtodomJsonParser
    {
        public Entry GetEntry([DisallowNull] String json)
        {
            Entry entry = new Entry();
            PropertyDetails propertyDetails = new PropertyDetails();
            PropertyPrice propertyPrice = new PropertyPrice();

            var otodomJsonSimple = JsonConvert.DeserializeObject<OtodomJsonSimple>(json);
            targetData = otodomJsonSimple.initialProps.meta.target;
            advertData = otodomJsonSimple.initialProps.data.advert;
            characteristicsData = advertData.characteristics;

            propertyDetails.Area = GetArea();
            propertyDetails.NumberOfRooms = GetRoomNumberOfRooms();
            propertyDetails.YearOfConstruction = GetBuildYear();
            propertyDetails.FloorNumber = GetFloorNumber();
            propertyDetails.Heating = GetHeating();
            propertyDetails.Url = GetUrl();

            propertyPrice.PricePerMeter = GetPricePerMeter();
            propertyPrice.TotalGrossPrice = GetPrice();

            OfferDetails offerDetails = new OfferDetails();
            offerDetails.IsStillValid = IsAdvertisementActive();
            offerDetails.Url = GetUrl();


            entry.PropertyDetails = propertyDetails;
            entry.PropertyPrice = propertyPrice;
            entry.OfferDetails = offerDetails;
            return entry;
        }

        OtodomJsonSimple.Target targetData;
        OtodomJsonSimple.Advert advertData;
        public OtodomJsonSimple.Characteristic[] characteristicsData { get; set; }
        Decimal GetArea()
        {
            if (null != targetData.Area)
            {
                return decimal.Parse(targetData.Area.Replace(".", ","));
            }
            return 0;
        }

        int GetRoomNumberOfRooms()
        {
            try
            {
                return int.Parse(targetData.Rooms_num[0] ?? "0");
            }
            catch(System.FormatException e)
            {
                //Sometimes Rooms_num is string like more, which says more than 10
                return 0;
            }
        }
        int GetBuildYear()
        {
            return int.Parse(targetData.Build_year ?? "0");
        }
        int GetPricePerMeter()
        {
            return targetData.Price_per_m;
        }
        int GetPrice()
        {
            return targetData.Price;
        }
        String GetFloorNumber()
        {
            if (characteristicsData.FirstOrDefault(data => data.key == "floor_no") != null)
            {
                return characteristicsData.FirstOrDefault(data => data.key == "floor_no").value_translated;
            }
            return "";
        }

        String GetHeating()
        {
            if (characteristicsData.FirstOrDefault(data => data.key == "heating") != null)
            {
                return characteristicsData.FirstOrDefault(data => data.key == "heating").value_translated;
            }
            return "";
        }

        bool IsAdvertisementActive()
        {
            if (advertData.status.Equals("active"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        String GetUrl()
        {
            return advertData.url;
        }

    }
}
