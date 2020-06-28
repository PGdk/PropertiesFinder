using DatabaseConnection;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Application.Trovit
{
     public interface ITrovitService {
        IEnumerable<Entry> GetTopOffers();
    }

    public class TrovitService : ITrovitService
    {
        private ITrovitRepository repository;
        private ITrovitScoreStrategy strategy;

        public TrovitService(ITrovitRepository repository, ITrovitScoreStrategy strategy) {
            this.repository = repository;
            this.strategy = strategy;
        }

        public IEnumerable<Entry> GetTopOffers()
        {
            return repository.GetEntries().Where(entry => strategy.Score(entry) > 80).OrderByDescending(entry => strategy.Score(entry)).Take(5).ToList();
        }
    }
}
