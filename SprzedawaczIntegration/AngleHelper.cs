using AngleSharp;
using AngleSharp.Dom;
using System;
using System.Collections.Generic;
using System.Text;

namespace SprzedawaczIntegration
{
    public class AngleHelper
    {
        public virtual IDocument GetParsedHtmlFromUrl(string url)
        {
            var config = Configuration.Default.WithDefaultLoader();
            var context = BrowsingContext.New(config);
            return context.OpenAsync(new Url(url)).Result;
        }
    }
}
