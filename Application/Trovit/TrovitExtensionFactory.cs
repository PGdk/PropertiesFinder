using Application.Trovit.Provider;
using Interfaces.Trovit;
using Models.Trovit;
using System;

namespace Application.Trovit
{
    internal class TrovitExtensionFactory 
    {
        public static ITrovitExtension New(TrovitEntry entry) {
            if (entry.ProviderDetails == null)
                return null;

            switch (entry.ProviderDetails.Domain) {
                case "tabelaofert.pl":
                    return new TabelaOfertExtension(entry.ProviderDetails.URL);
                default:
                    return null;
            }
        }
    }

}
