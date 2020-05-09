namespace Application.Utils
{
    /// <summary>
    /// Extension methods for easier string handling etc.
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Removes all white charactes: spces, new lines etc.
        /// </summary>
        /// <param name="s">Text to remove the whitespaces from.</param>
        /// <returns>Text without whitespaces.</returns>
        public static string RemoveAllWhiteSpaces(this string s)
            => s.Replace("\t", "").Replace("\n", "").Replace("\r", "").Replace(" ", "");

        /// <summary>
        /// Changes given text to upper latin letters ready to be parsed into PolishCity enum.
        /// </summary>
        /// <param name="s">String to parse.</param>
        /// <returns>String with upper latin letters only.</returns>
        public static string ToUpperLatinLetters(this string s)
            => s.ToUpper().Replace('Ó', 'O').Replace('Ł', 'L').Replace('Ć', 'C')
                .Replace('Ż', 'Z').Replace('Ź', 'Z').Replace('Ę', 'E').Replace('Ś', 'S')
                .Replace('Ń', 'N').Replace('Ą', 'A').Replace(' ', '_').Replace('-', '_');
    }
}
