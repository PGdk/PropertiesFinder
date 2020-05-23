using System.Linq;
using System.Text.RegularExpressions;

namespace Exhouse.Services
{
    public class NumberFromTextExtractor
    {
        public static decimal Extract(string text)
        {
            return decimal.Parse(
                Regex.Match(
                    string.Concat(text.Where(c => char.IsDigit(c) || ',' == c || '-' == c)).Replace(',', '.'),
                    @"(-)?\d+(\.\d+)?"
                ).Value
            );
        }
    }
}
