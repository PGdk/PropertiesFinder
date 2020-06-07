using AutoMapper;
using DatabaseConnection.Models;
using IntegrationApi.Repositories;
using NieruchomosciOnline;
using System;
using System.Collections.Generic;
using MEntry = Models.Entry;

namespace IntegrationApi.Services
{
    public class EntriesService : IEntriesService
    {
        private readonly IEntriesRepository entriesRepository;
        private readonly NieruchomosciOnlineIntegration nieruchomosciOnlineIntegration;
        private readonly IMapper mapper;
        private readonly IEqualityComparer<MEntry> equalityComparer;

        public bool CheckForDuplicates { get; set; } = true;

        public EntriesService(IEntriesRepository entriesRepository,
            NieruchomosciOnlineIntegration nieruchomosciOnlineIntegration,
            IMapper mapper,
            IEqualityComparer<MEntry> equalityComparer)
        {
            this.equalityComparer = equalityComparer ?? throw new ArgumentException("Argument cannot be null", nameof(equalityComparer));
            this.mapper = mapper ?? throw new ArgumentException("Argument cannot be null", nameof(mapper));
            this.entriesRepository = entriesRepository ?? throw new ArgumentException("Argument cannot be null", nameof(entriesRepository));
            this.nieruchomosciOnlineIntegration = nieruchomosciOnlineIntegration ?? throw new ArgumentException("Argument cannot be null", nameof(nieruchomosciOnlineIntegration));
        }
        public void AddEntriesFromPage(int page)
        {
            var entries = this.nieruchomosciOnlineIntegration.GetEntriesFromPage(page);
            if(entries.Count == 0)
            {
                throw new KeyNotFoundException();
            }

            if (this.CheckForDuplicates)
            {
                var toBeRemoved = new List<MEntry>(); 
                foreach (var e in entries)
                {
                    var phone = e?.OfferDetails?.SellerContact?.Telephone;
                    if (phone != null)
                    {
                        var offersFromSameSeller = this.entriesRepository.GetEntriesFromSellerWithNumber(phone);
                        foreach(var offer in offersFromSameSeller)
                        {
                            var mappedOffer = this.mapper.Map<MEntry>(offer);
                            if ((this.equalityComparer).Equals(mappedOffer, e))
                            {
                                toBeRemoved.Add(e);
                                break;
                            }
                        }
                    }
                }

                entries.RemoveAll(e => toBeRemoved.Contains(e));
            }

            var dbEntries = this.mapper.Map<List<Entry>>(entries);
            this.entriesRepository.AddEntries(dbEntries);
        }

        public List<Entry> GetEntries()
           => this.entriesRepository.GetEntries();


        public Entry GetEntry(long id)
           => this.entriesRepository.GetEntry(id);

        public List<Entry> GetEntries(int pageId, int pageLimit)
           => this.entriesRepository.GetEntries((pageId - 1) * pageLimit, pageId * pageLimit);

        public void UpdateEntry(long id, MEntry mEntry)
        {
            var entry = this.mapper.Map<Entry>(mEntry);
            this.entriesRepository.UpdateEntry(id, entry);
        }
    }
}
