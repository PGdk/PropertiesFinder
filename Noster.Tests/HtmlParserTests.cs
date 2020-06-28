using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Noster;
using Application;
using Models;

namespace Noster.Tests
{
    public class HtmlParserTests
    {
        [Fact]
        public void Should_GetRentalOfferKind()
        {
            // Arrange
            string pathToHtml = @"..\..\..\HtmlDocuments\OfferKind_Rental.html";
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.Load(pathToHtml);
            var expected = Models.OfferKind.RENTAL;

            // Act
            var actual = HtmlParser.GetOfferKind(htmlDocument);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Should_GetValidSellerContact()
        {
            // Arrange
            string pathToHtml = @"..\..\..\HtmlDocuments\OfferKind_Rental.html";
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.Load(pathToHtml);
            var expected = new SellerContact { Email = "biuro@noster-nieruchomosci.pl", Name = "Jadwiga Urbanowicz", Telephone = "530280180" };

            // Act
            var actual = HtmlParser.GetSellecContact(htmlDocument);

            // Assert
            Assert.Equal(expected.Email, actual.Email);
            Assert.Equal(expected.Name, actual.Name);
            Assert.Equal(expected.Telephone, actual.Telephone);
        }

        [Fact]
        public void Should_ReturnZero_When_NoNumbersInTotalGrossPriceInHtmlDocument()
        {
            // Arrange
            string pathToHtml = @"..\..\..\HtmlDocuments\OfferKind_Rental_NoPrice.html";
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.Load(pathToHtml);
            decimal expected = 0;

            // Act
            decimal actual = HtmlParser.GetTotalGrossPrice(htmlDocument);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Should_GetGardenArea_When_GardenIsAvailable()
        {
            // Arrange
            string pathToHtml = @"..\..\..\HtmlDocuments\Garden.html";
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.Load(pathToHtml);
            decimal? expected = 5;

            // Act
            decimal? actual = HtmlParser.GetGardenArea(htmlDocument);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Should_GetNull_When_GardenIsNotAvailable()
        {
            // Arrange
            string pathToHtml = @"..\..\..\HtmlDocuments\NoGarden.html";
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.Load(pathToHtml);
            decimal? expected = null;

            // Act
            decimal? actual = HtmlParser.GetGardenArea(htmlDocument);

            // Assert
            Assert.Equal(expected, actual);
        }
    }
}