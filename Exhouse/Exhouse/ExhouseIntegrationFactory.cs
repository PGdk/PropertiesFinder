using Exhouse.Exhouse.Comparers;
using Utilities;

namespace Exhouse.Exhouse
{
    public class ExhouseIntegrationFactory
    {
        public static ExhouseIntegration Create()
        {
            return new ExhouseIntegration(
                new DumpFileRepository(),
                new EntryComparer()
            );
        }
    }
}
