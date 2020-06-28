using Application.Trovit;
using DatabaseConnection;
using Models;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;

namespace TrovitTests
{
    public class TrovitServiceTests
    {

        private TrovitService service;
        private Mock<ITrovitRepository> repository;
        private Mock<ITrovitScoreStrategy> strategy;

        [SetUp]
        public void Setup()
        {
            repository = new Mock<ITrovitRepository>();
            strategy = new Mock<ITrovitScoreStrategy>();
            service = new TrovitService(repository.Object, strategy.Object);
        }

        [Test]
        public void TestTopOffers()
        {

            var entries = new List<Entry>();
            entries.Add(new Entry { ID = 1 });
            entries.Add(new Entry { ID = 2 });
            entries.Add(new Entry { ID = 3 });
            entries.Add(new Entry { ID = 4 });
            entries.Add(new Entry { ID = 5 });
            entries.Add(new Entry { ID = 6 });

            repository.Setup(m => m.GetEntries()).Returns(entries);
            strategy.SetupSequence(m => m.Score(new Entry { }))
                .Returns(60)
                .Returns(81)
                .Returns(85)
                .Returns(97)
                .Returns(94)
                .Returns(100);


            var expected = new List<Entry>();
            entries.Add(new Entry { ID = 6 });
            entries.Add(new Entry { ID = 4 });
            entries.Add(new Entry { ID = 5 });
            entries.Add(new Entry { ID = 3 });
            entries.Add(new Entry { ID = 2 });


            var offers = service.GetTopOffers();


            Assert.AreEqual(expected, offers);
            Assert.Pass();
        }
    }
}