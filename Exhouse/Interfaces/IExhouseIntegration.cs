using System.Collections.Generic;
using Interfaces;
using Models;

namespace Exhouse.Interfaces
{
    public interface IExhouseIntegration : IWebSiteIntegration
    {
        public List<Entry> FetchEntriesFromOffersPage(int pageNumber);
    }
}
