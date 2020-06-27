using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bazos
{

    public interface IDocumentChecker
    {
        public bool IsDocumentNull(HtmlDocument doc);
    }


    class DocumentChecker : IDocumentChecker
    {
        public bool IsDocumentNull(HtmlDocument doc)
        {
            if (doc == null)
                return true;
            else
                return false;
        }
    }
}
