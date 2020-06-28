using HtmlAgilityPack;
using Models;
using System.Collections.Generic;

namespace DziennikBaltycki.Interfaces
{
    public interface IDziennikBaltyckiIntegration
    {
        HtmlNode PobierzDokument(string url);
        List<string> PobierzLinkiDoMieszkan(List<string> linkiDoStron);
        List<string> PobiezLinkiDoStron();
        int? SprawdzIleMetrow(string czegoSzukamy, ref string opis);
        PropertyAddress ZwrocAdres(Dictionary<string, string> zbiorDanych);
        PropertyFeatures ZwrocCechy(HtmlNode htmlbody, Dictionary<string, string> zbiorDanych);
        PropertyPrice ZwrocDaneOWartosci(HtmlNode htmlbody, Dictionary<string, string> zbiorDanych);
        int ZwrocLiczbeStron(HtmlNode htmlbody);
        PolishCity ZwrocNazweMiasta(string miastoPoKonversji);
        string ZwrocOpis(HtmlNode htmlbody);
        int? ZwrocParkingWewnetrzny(Dictionary<string, string> zbiorDanych, string opis);
        int? ZwrocParkingZewnetrzny(Dictionary<string, string> zbiorDanych);
        string ZwrocPojedynczeOgloszenieMieszkaniowe(HtmlNode linkA);
        OfferDetails ZwrocSzczegolyOferty(HtmlNode htmlbody, string url);
        PropertyDetails ZwrocSzczegolyWlasnosci(Dictionary<string, string> zbiorDanych);
        Dictionary<string, string> ZwrocZbiorDanych(HtmlNode htmlbody);
    }
}