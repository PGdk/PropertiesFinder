using System.Collections.Generic;
using System.Linq;
using DatabaseConnection;
using IntegrationApi.Services;
using Models;
using Moq;
using NUnit.Framework;

namespace GazetaKrakowskaUnitTests
{
    public class GazetaKrakowskaServiceTest
    {
        private GazetaKrakowskaService gazetaKrakowskaService;
        private Mock<IGazetaKrakowskaRepository> mockRepository;

        [SetUp]
        public void Setup()
        {
            //Arrange
            mockRepository = new Mock<IGazetaKrakowskaRepository>();
            gazetaKrakowskaService = new GazetaKrakowskaService(mockRepository.Object);
        }

        [Test]
        public void Test1()
        {
            //Arrange
            IEnumerable<EntryDb> mockEntries = new List<EntryDb>();
            mockRepository.Setup(m => m.GetEntries()).Returns(mockEntries);

            //Act
            IEnumerable<EntryDb> specialEntries = gazetaKrakowskaService.GetSpecialEntries();

            //Assert
            Assert.IsTrue(specialEntries.ToList().Count == 0);
        }
    }
}
