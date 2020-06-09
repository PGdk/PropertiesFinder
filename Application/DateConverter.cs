using System;
using System.Collections.Generic;
using System.Text;

namespace Application
{
    class DateConverter
    {
        public static string[] months = new string[] { " Styczeń, ", " Luty, ", " Marzec, ", " Kwiecień, ", " Maj, ", " Czerwiec, ", " Lipiec, ", " Sierpień, ", " Wrzesień, ", " Październik, ", " Listopad, ", " Grudzień, " };
        public static DateTime Convert(string date)
        {
            date = date.Replace("Dodane: ", "");

            date = date.Replace("Poniedziałek, ", "");
            date = date.Replace("Wtorek, ", "");
            date = date.Replace("Środa, ", "");
            date = date.Replace("Czwartek, ", "");
            date = date.Replace("Piątek, ", "");
            date = date.Replace("Sobota, ", "");
            date = date.Replace("Niedziela, ", "");

            for (int i = 0; i < months.Length; i++)
                date = date.Replace(months[i], '/' + (i + 1).ToString() + '/');

            date = date.Remove(date.IndexOf("&"), 6);
            return DateTime.Parse(date);
        }
    }
}
