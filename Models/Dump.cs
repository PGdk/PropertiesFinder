using System.Collections.Generic;
using System.Collections;
using System.Linq;

namespace Models
{
    /// <summary>
    /// Klasa reprezentująca wynik jednego zrzutu danych ze strony.
    /// </summary>
    public class Dump : DumpDetails
    {
        /// <summary>
        /// Kolekcja reprezentująca wszystkie oferty znalezione podczas zrzutu. Te dane powinny zostać pobrane automatycznie
        /// za pomocą parsera.
        /// </summary>
        
        public List<Entry> myEntries { get; set; }
    }
}
