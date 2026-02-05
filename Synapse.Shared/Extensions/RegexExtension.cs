using System.Text.RegularExpressions;

namespace Synapse.Shared.Extensions
{
    public static class RegexExtension
    {
        public static Regex AsRegex(this string pattern)
        {
            return new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
        }

        public static Regex AsRegex(this string pattern, RegexOptions options)
        {
            return new Regex(pattern, options);
        }

        public static bool RegexMatching(this string text, string regexMatchExpression) => text.RegexMatchExtractFirstValue(regexMatchExpression).HasValue();

        public static string RegexMatchExtractFirstValue(this string text, string regexMatchExpression)
        {
            return Regex.Match(text, regexMatchExpression).Value;
        }

        public static string RegexMatchGroupValue(this string text, string regexMatchExpression, int groupValue)
        {
            return Regex.Match(text, regexMatchExpression).Groups[groupValue].Value;
        }
    }
}
