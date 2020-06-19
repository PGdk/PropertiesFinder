using System.Collections.Generic;
using Microsoft.AspNetCore.TestHost;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Models;
using Newtonsoft.Json;
using Xunit;

namespace IntegrationApi.Tests
{
    public class EntriesDealsTests
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;
        public EntriesDealsTests()
        {
            // Arrange
            _server = new TestServer(new WebHostBuilder()
                .UseStartup<Startup>());
            _client = _server.CreateClient();
        }
        [Theory]
        [InlineData(812)]
        [InlineData(314)]
        public async Task GetSqmDeal_Warsaw_FoundEntries(int cityId)
        {
            // Act
            var response = await _client.GetAsync("entries/sqm-deal/" + cityId);
            var responseString = await response.Content.ReadAsStringAsync();
            var entries = JsonConvert.DeserializeObject<List<Entry>>(responseString);

            // Assert
            Assert.NotEmpty(entries);
            Assert.Equal(5, entries.Count);
        }
        [Fact]
        public async Task GetSqmDeal_NonExistingCity_NoEntries()
        {
            var response = await _client.GetAsync("entries/sqm-deal/-1");
            var responseString = await response.Content.ReadAsStringAsync();
            var entries = JsonConvert.DeserializeObject<List<Entry>>(responseString);
            Assert.Empty(entries);
        }
    }
}
