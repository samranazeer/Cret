using System.ComponentModel;
using System.Text.RegularExpressions;

namespace CRET.Domain.Helper
{
    public static class Utilities
    {
        public static string GetDescription(this System.Enum enumValue)
        {
            //Look for DescriptionAttributes on the enum field
            object[] attr = enumValue.GetType().GetField(enumValue.ToString())
                .GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attr.Length > 0) // a DescriptionAttribute exists; use it
                return ((DescriptionAttribute)attr[0]).Description;

            //The above code is all you need if you always use DescriptionAttributes;

            return enumValue.ToString();
        }

        public static string ExtractCertificate(string input)
        {
            // Define a regular expression pattern to match PEM format certificates
            string pattern = @"(-----BEGIN CERTIFICATE-----(.*?)-----END CERTIFICATE-----)";

            // Match the pattern in the input string
            Match match = Regex.Match(input, pattern, RegexOptions.Singleline);

            // If a match is found, extract and return the certificate content
            if (match.Success)
            {
                return match.Groups[1].Value.Trim();
            }
            else
            {
                // If no match is found, return null or an empty string, depending on your requirement
                return string.Empty;
            }
        }

        public static string ExtractCSR(string input)
        {
            // Define a regular expression pattern to match PEM format certificates
            string pattern = @"(-----BEGIN CERTIFICATE REQUEST-----(.*?)-----END CERTIFICATE REQUEST-----)";

            // Match the pattern in the input string
            Match match = Regex.Match(input, pattern, RegexOptions.Singleline);

            // If a match is found, extract and return the certificate content
            if (match.Success)
            {
                return match.Groups[1].Value.Trim();
            }
            else
            {
                // If no match is found, return null or an empty string, depending on your requirement
                return string.Empty;
            }
        }
    }
}
