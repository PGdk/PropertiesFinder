using System;
using System.Collections.Generic;
using System.Text;

namespace Bazos
{
    public interface IPolishStringParser
    {
        public string ChangePolishCharacters(string s);
    }


    class PolishStringParser : IPolishStringParser
    {
        public string ChangePolishCharacters(string s) //Usuwanie polskich znaków z nazwy miasta
        {
            s = s.ToUpper();
            string[,] exchangeableChar =
            {
                { "Ą", "A" }, { "Ć", "C" }, { "Ę", "E" }, { "Ł", "L" }, { "Ń", "N" }, { "Ó", "O" }, { "Ś", "S" }, { "Ź", "Z" }, { "Ż", "Z" }
            };
            for (int i = 0; i < exchangeableChar.GetLength(0); i++)
            {
                s = s.Replace(exchangeableChar[i, 0], exchangeableChar[i, 1]);
            }

            return s;
        }
    }
}
