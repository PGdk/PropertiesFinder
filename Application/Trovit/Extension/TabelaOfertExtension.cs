using Application.Trovit.Enhance;
using Application.Trovit.Parser;

using Interfaces.Trovit;
using System;
using Utilities.Trovit;
namespace Application.Trovit.Provider
{
    internal class TabelaOfertExtension : ITrovitExtension
    {

        private string URL;

        public TabelaOfertExtension(string URL) {
            this.URL = URL;
        }
        
        public ITrovitEnhancer Handle()
        {
            var content = HTTPClient.Get(URL);

            if (content == null) 
                return null;

            var details = new TabelaOfertParser().Parse(content);

            if (details == null) 
                return null;

            return new TabelaOfertEnhancer(details);
        }
    }
}
