using Interfaces.Trovit;

namespace Application.Trovit.Parser
{

    internal class TabelaOfertDetails
    {
        public string SellerTelephone { get; set; }
        public string SellerName { get; set; }
    }


    internal class TabelaOfertParser : ITrovitParser<TabelaOfertDetails>
    {
        public TabelaOfertDetails Parse(string content)
        {
            content = content.Replace("\n", "").Replace("\r", "");

            var sellerTelephone = ExtractSellerTelephone(content);
            var sellerName = ExtractSellerName(content);

            if (sellerName == "" && sellerTelephone == "")
                return null;

            if (sellerName == "")
                sellerName = null;

            if (sellerTelephone == "")
                sellerName = null;

            return new TabelaOfertDetails
            {
                SellerTelephone = sellerTelephone,
                SellerName = sellerName,
            };
        }

        private string ExtractSellerTelephone(string s) {
            return extract(s, "data-telefon=\"", "\"");
        }

        private string ExtractSellerName(string s) {
            return extract(s, "target=\"_blank\">", "</a></h4>");
        }

        private string extract(string s, string begining, string ending)
        {
            if (s.IndexOf(begining) == -1)
                return "";

            s = s.Substring(s.IndexOf(begining) + begining.Length);

            if (s.IndexOf(ending) == -1)
                return "";

            s = s.Substring(0, s.IndexOf(ending));

            return s;
        }
    }
}
