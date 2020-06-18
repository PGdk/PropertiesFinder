using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace nportal.pl
{
    public static class Extensions
    {
        public static DateTime ToDateTime(this string dt)
        {
            var dts = dt.Split(" ");
            var dts2 = dts[2].Split("-");
            var year = Convert.ToInt32(dts2[0]);
            var month = Convert.ToInt32(dts2[1]);
            var day = Convert.ToInt32(dts2[2]);
            if (dts.Length > 3)
            {
                var dts3 = dts[3].Split(":");
                var hour = Convert.ToInt32(dts3[0]);
                var minute = Convert.ToInt32(dts3[1]);
                var second = Convert.ToInt32(dts3[2]);
                return new DateTime(year, month, day, hour, minute, second);
            }
            return new DateTime(year, month, day);
        }
        public static string UpperCaseFirstLetter(this string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }
            return char.ToUpper(text[0]) + text.Substring(1);
        }
        public static string RemoveDiacritics(this string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return text;

            text = text.Normalize(NormalizationForm.FormD);
            var chars = text.Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark).ToArray();
            return new string(chars).Normalize(NormalizationForm.FormC);
        }
    }
}
