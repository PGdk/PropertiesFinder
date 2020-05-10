using HtmlAgilityPack;

namespace EchodniaEu
{
    class Page
    {
        protected string Url { get; set; }

        protected HtmlDocument HtmlDocument { get; set; }

        public Page(string url)
        {
            Url = url;
            HtmlDocument = new HtmlWeb().Load(url);
        }
    }
}
