using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Troonie_Lib
{
    public static class StringHelper
    {
        /// <summary>
        /// Replaces german umlauts from given string and returns the result string.
        /// </summary>
        /// <remarks>
        /// Source: https://stackoverflow.com/questions/1271567/how-do-i-replace-accents-german-in-net/1271695#1271695
        /// </remarks>
        /// <param name="input">Input string, which contains german umlauts to replace.</param>
        /// <returns>Input string with replaced german umlauts.</returns>
        public static string ReplaceGermanUmlauts(string input)
        {
            var map = new Dictionary<char, string>() 
            {
                { 'ä', "ae" },
                { 'ö', "oe" },
                { 'ü', "ue" },
                { 'Ä', "Ae" },
                { 'Ö', "Oe" },
                { 'Ü', "Ue" },
                { 'ß', "ss" }
            };

            string result = input.Aggregate(
                          new StringBuilder(),
                          (sb, c) => map.TryGetValue(c, out string r) ? sb.Append(r) : sb.Append(c)
                          ).ToString();
            
            return result;
        }
    }
}
