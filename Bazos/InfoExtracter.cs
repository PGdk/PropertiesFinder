﻿using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Diagnostics.CodeAnalysis;

namespace Bazos
{
    public class InfoExtracter
    {
        public IPolishStringParser polishStringParser  {get; set;}
        public IDocumentChecker documentChecker { get; set; }

        public InfoExtracter(IPolishStringParser parser, IDocumentChecker checker)
        {
            polishStringParser = parser;
            documentChecker = checker;
        }

        public bool ExtractInfoFromPropertyPage(Dictionary<string, string> info, HtmlDocument doc)
        {
            bool isNull = documentChecker.IsDocumentNull(doc);
            if (isNull)
            {
                return false;
            }
            
            //Zbieramy info z poszczególnych elementów strony
            try
            {
                ExtractTopBar(info, doc);
                ExtractLeftColumn(info, doc);
                ExtractDescription(info, doc);
            }
            catch (NullReferenceException nre)
            {
                Console.WriteLine("Exception: " + nre);
            }
            return true;
        }

        private void ExtractDescription(Dictionary<string, string> info, HtmlDocument doc)
        {
            //Zbieram wszystkie wyrazy z opisu i konwertuje je na listę
            var dataInfo = doc.DocumentNode.SelectNodes("//div[@class=\"popis\"]");

            var listInfo = new List<string>();
            if (dataInfo != null)
            {
                ExtractInnerText(dataInfo, listInfo);
            }

            char[] separators = { ' ', '.', ',', '!', '?', '-' };
            var description = listInfo[0].Split(separators);
            List<string> descList = description.ToList<string>();

            for (int i = 0; i < descList.Count;)
            {
                if (descList[i] == "")
                {
                    descList.Remove(descList[i]);
                }
                else
                {
                    i++;
                }
            }

            //Zapobiegam wychodzeniu poza range na liście z wyrazami z opisu
            descList.Insert(0, "Start");
            descList.Add("End");
            ExtractInfoFromDescription(info, descList);
        }

        private void ExtractInfoFromDescription(Dictionary<string, string> info, List<string> descList)
        {
            int rent = 0;
            bool roomInfo = false, areaInfo = false;

            //Dla każego wyrazu sprawdzamy czy nie wpisuje się w daną informację
            for (int i = 0; i < descList.Count; i++)
            {
                var currentString = polishStringParser.ChangePolishCharacters(descList[i]);
                //GardenArea
                if (currentString.Contains("OGROD"))
                {
                    info["GardenArea"] = "1";
                    string dictName = "GardenArea";
                    int descListElement = i;
                    CheckArea(info, descList, dictName, descListElement);
                }
                //Balconies
                else if (currentString.Contains("BALKON"))
                {
                    info["Balconies"] = "1";
                    if (descList[i - 1].All(char.IsDigit))
                        info["Balconies"] = descList[i - 1];
                }
                //BasementArea
                else if (currentString.Contains("PIWNICA"))
                {
                    info["BasementArea"] = "1";
                    string dictName = "BasementArea";
                    int descListElement = i;
                    CheckArea(info, descList, dictName, descListElement);
                }
                //OutdoorParkingPlaces
                else if (currentString.Contains("PARKING") || currentString.Contains("POSTOJ"))
                {
                    string parkingPlace = "a";
                    int descListElement = i;
                    if (polishStringParser.ChangePolishCharacters(descList[i - 1]).Contains("PODZIEMNY"))
                    {
                        info["IndoorParkingPlaces"] = "1";
                        descListElement = i - 1;
                        parkingPlace = "IndoorParkingPlaces";
                        NumberOfParkingPlaces(info, descList, descListElement, parkingPlace); //Sprawdzam kilka wyrazów wstecz o ilość miejsc parkingowych
                    }
                    else if (polishStringParser.ChangePolishCharacters(descList[i + 1]).Contains("PODZIEMNY"))
                    {
                        info["IndoorParkingPlaces"] = "1";
                        parkingPlace = "IndoorParkingPlaces";
                        NumberOfParkingPlaces(info, descList, descListElement, parkingPlace);
                    }
                    else
                    {
                        info["OutdoorParkingPlaces"] = "1";
                        parkingPlace = "OutdoorParkingPlaces";
                        NumberOfParkingPlaces(info, descList, descListElement, parkingPlace);
                    }
                }
                //IndoorParkingPlaces
                else if (currentString.Contains("GARAZ"))
                {
                    info["IndoorParkingPlaces"] = "1";
                    string parkingPlace = "IndoorParkingPlaces";
                    NumberOfParkingPlaces(info, descList, i, parkingPlace);
                }
                //StreetName
                else if (currentString == "UL")
                {
                    info["StreetName"] = descList[i + 1];
                    if(i+2<descList.Count-1)
                    {
                        if (descList[i + 2].Any(Char.IsDigit))
                            info["DetailedAddress"] =polishStringParser.ChangePolishCharacters(descList[i + 2]) + ", " + info["DetailedAddress"];
                    }
                }
                //Area
                else if (currentString == "M" && !areaInfo)
                {
                    if (descList[i - 1].All(char.IsDigit))
                    {
                        info["Area"] = descList[i - 1];
                        areaInfo = true;
                    }
                }
                else if (currentString.Contains("MKW") && !areaInfo)
                {
                    if (descList[i - 1].All(char.IsDigit))
                    {
                        info["Area"] = descList[i - 1];
                        areaInfo = true;
                    }
                }
                else if (currentString.EndsWith("M") && !areaInfo)
                {
                    var substring = descList[i].Substring(0, descList[i].Length - 1);
                    if (substring.All(char.IsDigit))
                    {
                        info["Area"] = substring;
                        areaInfo = true;
                    }
                }
                //NumberOfRooms
                else if (currentString.Contains("POKOI") || currentString.Contains("POKOJ"))
                {
                    if (descList[i - 1].All(char.IsDigit))
                    {
                        info["NumberOfRooms"] = descList[i - 1];
                        roomInfo = true;
                    }
                    else if (!roomInfo)
                        info["NumberOfRooms"] = "1";
                }
                else if (currentString.Contains("KAWALERK"))
                {
                    info["NumberOfRooms"] = "1";
                }
                //FloorNumber
                else if (currentString.Contains("PIETR"))
                {
                    if (descList[i - 1].All(char.IsDigit))
                        info["FloorNumber"] = descList[i - 1];
                    else if (descList[i + 1].All(char.IsDigit))
                        info["FloorNumber"] = descList[i + 1];
                }
                else if (currentString.Contains("PARTER"))
                {
                    info["FloorNumber"] = "0";

                }
                //ResidentalRent
                else if (currentString == "ZL")
                {
                    if (descList[i - 1].All(char.IsDigit))
                        rent += Convert.ToInt32(descList[i - 1]);
                }
                else if (currentString.EndsWith("ZL"))
                {
                    var substring = descList[i].Substring(0, descList[i].Length - 2);
                    if (substring.All(char.IsDigit))
                        rent += Convert.ToInt32(substring);
                }
                //YearOfConstruction
                else if (currentString == "R")
                {
                    if (descList[i - 1].All(char.IsDigit))
                        info["YearOfConstruction"] = descList[i - 1];
                }
                else if (currentString.EndsWith("R"))
                {
                    var substring = descList[i].Substring(0, descList[i].Length - 1);
                    if (substring.All(char.IsDigit))
                        info["YearOfConstruction"] = substring;
                }
                else if (currentString.Contains("@"))
                {
                    info["Email"] = currentString + "." + polishStringParser.ChangePolishCharacters(descList[i + 1]);
                }
                else if (currentString.Contains("MIEJSCOWOSC"))
                {
                    info["District"] = descList[i+1];
                }
            }
            //Rent
            if (rent != 0)
                info["ResidentalRent"] = rent.ToString();
        }

        private void ExtractTopBar(Dictionary<string, string> info, HtmlDocument doc)
        {
            //Creation Date Time
            var dataInfo = doc.DocumentNode.SelectNodes("//span[@class=\"velikost10\"]");

            var listInfo = new List<string>();
            if (dataInfo != null)
            {
                ExtractInnerText(dataInfo, listInfo);
                string creationDate = "";
                if (listInfo.Count > 2)
                {
                    for(int i=0; i<listInfo.Count; i++)
                    {
                        if (listInfo[i] == "TOP")
                        {
                            creationDate = listInfo[i+1];
                            break;
                        }
                        if (i == listInfo.Count-1)
                        {
                            creationDate = listInfo[0];
                        }
                    }
                }

                else
                    creationDate = listInfo[0];
                char[] separators = { '[', '.', ' ', ']' };
                var creationStrings = creationDate.Split(separators);
                creationDate = creationStrings[3] + "-" + creationStrings[4] + "-" + creationStrings[6];
                info["CreationDateTime"] = creationDate;
            }

            //Rental or Sale
            dataInfo = doc.DocumentNode.SelectNodes("//div[@class=\"drobky\"]");

            listInfo = new List<string>();
            if (dataInfo != null)
            {
                ExtractInnerText(dataInfo, listInfo);
                if (listInfo.Contains("Na wynajem"))
                    info["Rental"] = "WYNAJEM";
                else
                    info["Rental"] = "SPRZEDAZ";
            }

            //Raw description info
            var rawDescriptionInfo = doc.DocumentNode.SelectNodes("//h1[@class=\"nadpis\"]");

            listInfo = new List<string>();
            if (rawDescriptionInfo != null)
            {
                ExtractInnerText(rawDescriptionInfo, listInfo);
                info["RawDescription"] = listInfo[0];
            }
        }

        private void ExtractLeftColumn(Dictionary<string, string> info, HtmlDocument doc)
        {
            //Name, telephone, Detailed Address, City, TotalGrossPrice
            var contactInfo = doc.DocumentNode.SelectNodes("//td[@class=\"listadvlevo\"]");
            var listInfo = new List<string>();
            if (contactInfo != null)
            {
                ExtractInnerText(contactInfo, listInfo);
                //wyrzucam niepotrzebne wyrazy
                for (int i = 0; i < listInfo.Count;)
                {
                    if (listInfo[i].Contains("Imię:") || listInfo[i].Contains("Telefon:") || listInfo[i].Contains("Lokalizacja:") || listInfo[i].Contains("Widziało:") || listInfo[i].Contains("Cena:"))
                    {
                        listInfo.Remove(listInfo[i]);
                    }
                    else
                    {
                        i++;
                    }
                }

                info["Name"] = listInfo[0];
                info["Telephone"] = listInfo[1];
                var split = listInfo[2].Split(" ");
                info["DetailedAddress"] = split[0];
                info["City"] = polishStringParser.ChangePolishCharacters(split[1]);

                char[] separators = { ' ', 'z', 'l', 'ł' };
                string totalGrossPrice = listInfo[4].Trim(separators);

                totalGrossPrice = totalGrossPrice.Replace(" ", string.Empty);

                if (totalGrossPrice.All(char.IsDigit))
                    info["TotalGrossPrice"] = totalGrossPrice;
            }
        }

        public void ExtractInnerText(HtmlNodeCollection nodeCol, List<string> info)
        {
            foreach (HtmlNode node in nodeCol)
            {
                if (node.ChildNodes.Count != 0)
                {
                    var newNodeCol = node.ChildNodes;
                    ExtractInnerText(newNodeCol, info);
                }
                else
                {
                    if (node.InnerText != "\r\n" && node.InnerText != "")
                        info.Add(node.InnerText);
                }
            }
        }

        public void NumberOfParkingPlaces(Dictionary<string, string> info, List<string> descList, int descListElement, string parkingPlace)
        {
            try
            {
                for (int j = 0; j < 2; j++)
                {
                    descListElement = descListElement - 1;
                    var currentString = polishStringParser.ChangePolishCharacters(descList[descListElement]);
                    if (currentString.Contains("MIEJSC"))
                    {
                        if (descList[descListElement - 1].All(char.IsDigit))
                        {
                            info[parkingPlace] = descList[descListElement - 1];
                        }
                    }
                }
            }
            catch(System.ArgumentOutOfRangeException aor)
            {
                Console.WriteLine($"Out Of Range! {aor}");
                throw;
            }
            catch(System.Exception e)
            {
                Console.WriteLine($"Exception! {e}");
            }
        }

        public void CheckArea(Dictionary<string, string> info, List<string> descList, string dictName, int descListElement) //Sprawdzam czy znajde wielkosc pomieszczenia wokol slowa klucza
        {
            descListElement = descListElement + 1;
            try
            {
                for (int j = 0; j <= 3; j++)
                {
                    string stringFromDesc = polishStringParser.ChangePolishCharacters(descList[descListElement]);
                    if (stringFromDesc == "M")
                    {
                        if (descList[descListElement - 1].All(char.IsDigit))
                            info[dictName] = descList[descListElement - 1];
                    }
                    else if (stringFromDesc.EndsWith("M"))
                    {
                        var substring = stringFromDesc.Substring(0, stringFromDesc.Length - 1);
                        if (substring.All(char.IsDigit))
                            info[dictName] = substring;
                    }
                    descListElement--;
                }
            }
            catch(System.ArgumentOutOfRangeException aor)
            {
                Console.WriteLine($"Out Of Range! {aor}");
                throw;
            }
            catch(System.Exception e)
            {
                Console.WriteLine($"Exception! {e}");
            }
        }
    }
}

