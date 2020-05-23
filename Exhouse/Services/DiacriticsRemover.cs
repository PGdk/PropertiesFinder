using System.Globalization;
using System.Text;

namespace Exhouse.Services
{
    public class DiacriticsRemover
    {
        public static string Remove(string text)
        {
            StringBuilder stringBuilder = new StringBuilder();

            string normalizedText = text.Normalize(NormalizationForm.FormD);

            foreach (char c in normalizedText)
            {
                UnicodeCategory unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);

                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}
