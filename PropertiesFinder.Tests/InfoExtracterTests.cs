using Models;
using NUnit.Framework;
using System.Collections.Generic;
using Bazos;
using Moq;

namespace PropertiesFinder.Tests
{
    // Projekt 3 Podpunkt 3
    [TestFixture]
    class InfoExtracterTests
    {
        public Mock<IPolishStringParser> _polishStringParserMock;
        public IPolishStringParser parser;
        public InfoExtracter infoExtracter;

        [SetUp]
        public void SetUp()
        {
            _polishStringParserMock = new Mock<IPolishStringParser>();
            parser = _polishStringParserMock.Object;
            infoExtracter = new InfoExtracter(parser);
        }

        [Test]
        public void NumberOfParkingPlaces_NumberFound_infoParkingPlaceIsChanged()
        {
            //Arrange
            Dictionary<string, string> info = new Dictionary<string, string>();
            info.Add("parkingPlace", "1");

            List<string> descList = new List<string>();
            descList.Add("3");  
            descList.Add("2");
            descList.Add("1");

            _polishStringParserMock
            .Setup(x => x.ChangePolishCharacters(It.IsAny<string>()))
            .Returns("MIEJSC");


            //Act
            infoExtracter.NumberOfParkingPlaces(info, descList, 3, "parkingPlace");

            //Assert
            Assert.IsTrue(info["parkingPlace"]=="3");
        }

        [Test]
        public void NumberOfParkingPlaces_NumberNotFound_infoParkingPlaceIsTheSame()
        {
            //Arrange
            Dictionary<string, string> info = new Dictionary<string, string>();
            info.Add("parkingPlace", "1");

            List<string> descList = new List<string>();
            descList.Add("3");
            descList.Add("2");
            descList.Add("1");

            _polishStringParserMock
            .Setup(x => x.ChangePolishCharacters(It.IsAny<string>()))
            .Returns("NULL");

            //Act
            infoExtracter.NumberOfParkingPlaces(info, descList, 3, "parkingPlace");

            //Assert
            Assert.IsTrue(info["parkingPlace"] == "1");
        }

        [Test]
        public void NumberOfParkingPlaces_DescListTooSmall_ThrowsArgumentOutOfRangeException()
        {
            //Arrange
            Dictionary<string, string> info = new Dictionary<string, string>();
            info.Add("parkingPlace", "1");

            List<string> descList = new List<string>();
            descList.Add("1");

            _polishStringParserMock
            .Setup(x => x.ChangePolishCharacters(It.IsAny<string>()))
            .Returns("MIEJSC");

            //Act + Assert
            Assert.Throws<System.ArgumentOutOfRangeException>(() => infoExtracter.NumberOfParkingPlaces(info, descList, 3, "parkingPlace"));
        }

        [Test]
        public void NumberOfParkingPlaces_DescListElementTooSmall_ThrowsArgumentOutOfRangeException()
        {
            //Arrange
            Dictionary<string, string> info = new Dictionary<string, string>();
            info.Add("parkingPlace", "1");

            List<string> descList = new List<string>();
            descList.Add("3");
            descList.Add("2");
            descList.Add("1");

            _polishStringParserMock
            .Setup(x => x.ChangePolishCharacters(It.IsAny<string>()))
            .Returns("MIEJSC");

            //Act + Assert
            Assert.Throws<System.ArgumentOutOfRangeException>(() => infoExtracter.NumberOfParkingPlaces(info, descList, 0, "parkingPlace"));
        }

        [Test]
        public void CheckArea_NumberFoundWithoutM_infoAreaIsChanged()
        {
            //Arrange
            Dictionary<string, string> info = new Dictionary<string, string>();
            info.Add("GardenArea", "1");

            List<string> descList = new List<string>();
            descList.Add("400");
            descList.Add("300");
            descList.Add("200");
            descList.Add("100");

            _polishStringParserMock
            .Setup(x => x.ChangePolishCharacters(It.IsAny<string>()))
            .Returns("100M");

            //Act
            infoExtracter.CheckArea(info, descList, "GardenArea", 2);

            //Assert
            Assert.IsTrue(info["GardenArea"] != "1");
        }

        [Test]
        public void CheckArea_NumberFoundWithM_infoAreaIsChanged()
        {
            //Arrange
            Dictionary<string, string> info = new Dictionary<string, string>();
            info.Add("GardenArea", "1");

            List<string> descList = new List<string>();
            descList.Add("500");
            descList.Add("400");
            descList.Add("300");
            descList.Add("200");
            descList.Add("100");

            _polishStringParserMock
            .Setup(x => x.ChangePolishCharacters(It.IsAny<string>()))
            .Returns("M");

            //Act
            infoExtracter.CheckArea(info, descList, "GardenArea", 3);

            //Assert
            Assert.IsTrue(info["GardenArea"] != "1");
        }

        [Test]
        public void CheckArea_NumberNotFound_infoParkingPlaceIsTheSame()
        {
            //Arrange
            Dictionary<string, string> info = new Dictionary<string, string>();
            info.Add("GardenArea", "1");

            List<string> descList = new List<string>();
            descList.Add("500");
            descList.Add("400");
            descList.Add("300");
            descList.Add("200");
            descList.Add("100");

            _polishStringParserMock
            .Setup(x => x.ChangePolishCharacters(It.IsAny<string>()))
            .Returns("NULL");


            //Act
            infoExtracter.CheckArea(info, descList, "GardenArea", 3);

            //Assert
            Assert.IsTrue(info["GardenArea"] == "1");
        }

        [Test]
        public void CheckArea_DescListTooSmall_ThrowsArgumentOutOfRangeException()
        {
            //Arrange
            Dictionary<string, string> info = new Dictionary<string, string>();
            info.Add("GardenArea", "1");

            List<string> descList = new List<string>();
            descList.Add("1");

            _polishStringParserMock
            .Setup(x => x.ChangePolishCharacters(It.IsAny<string>()))
            .Returns("M");

            //Act + Assert
            Assert.Throws<System.ArgumentOutOfRangeException>(() => infoExtracter.CheckArea(info, descList, "GardenArea", 2));
        }

        [Test]
        public void CheckArea_DescListElementTooSmall_ThrowsArgumentOutOfRangeException()
        {
            //Arrange
            Dictionary<string, string> info = new Dictionary<string, string>();
            info.Add("GardenArea", "1");

            List<string> descList = new List<string>();
            descList.Add("500");
            descList.Add("400");
            descList.Add("300");
            descList.Add("200");
            descList.Add("100");

            _polishStringParserMock
            .Setup(x => x.ChangePolishCharacters(It.IsAny<string>()))
            .Returns("M");

            //Act + Assert
            Assert.Throws<System.ArgumentOutOfRangeException>(() => infoExtracter.CheckArea(info, descList, "GardenArea", 0));
        }
    }
}
