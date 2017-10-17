using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Xml;
using System.Linq;
using System.IO;
using System.Data;
using System.Globalization;

namespace StringUtils
{
    /// <summary>
    /// Static class with some Newline String utilities
    /// </summary>
    static class Newlines
    {
        /// <summary>
        /// Adjust any kind of newline convention in a string (\r\n, \r, \n) to the desired newline string.
        /// If omitted, the environment newline is used.
        /// </summary>
        /// <param name="s">String in which the newlines are to be adjusted.</param>
        /// <param name="newLine">Desired newline string (if omitted or null, the environment newline is used).</param>
        /// <returns>The original string with adjusted newlines.</returns>
        public static String adjustNewlines(String s, String newLine = null)
        {
            if (newLine == null)
                newLine = Environment.NewLine;
            if (s != null)
                return Regex.Replace(s, @"(\r\n?)|\n", newLine);
            else
                return s;
        }

        /// <summary>
        /// Escapes newlines in a string, displaying them by showing a code for them instead.
        /// </summary>
        /// <param name="s">The string in which to escape newlines.</param>
        /// <param name="representation">The representation string for a newline.</param>
        /// <returns>The string, with newlines replaces by a representation string.</returns>
        public static String escapeNewlines(String s, String representation = @"\n")
        {
            return adjustNewlines(s, representation);
        }

        /// <summary>
        /// Array of newline sequences which can be used in <c>String.Split</c> to split a string on newlines
        /// </summary>
        private static String[] newLines = new String[] { "\n", "\r\n" };

        /// <summary>
        /// Split a (multiline) String into lines and remove empty entries (by default).
        /// </summary>
        /// <param name="s">The (multiline) String to be split.</param>
        /// <param name="splitOptions"><c>StringSplitOptions</c>, removing empty entries by default.</param>
        /// <returns>An array of Strings with an entry for each line in the original String.</returns>
        public static String[] splitLines(String s, StringSplitOptions splitOptions = StringSplitOptions.RemoveEmptyEntries)
        {
            return s.Split(newLines, splitOptions);
        }
    }

    /// <summary>
    /// Static class with some XML String utilities
    /// </summary>
    static class XmlHelp
    {
        /// <summary>
        /// Method that escaped special characters in a string for XML.
        /// </summary>
        /// <param name="unescaped">The original, unescaped string.</param>
        /// <returns>The escaped string.</returns>
        public static String XmlEscape(String unescaped)
        {
            XmlDocument doc = new XmlDocument();
            var node = doc.CreateElement("root");
            node.InnerText = unescaped;
            return node.InnerXml;
        }

        /// <summary>
        /// Method that escaped special characters in a string for XML.
        /// </summary>
        /// <param name="unescaped">The original, unescaped string.</param>
        /// <returns>The escaped string.</returns>
        public static String XmlEscapeWriter(String unescaped)
        {
            StringBuilder sb = new StringBuilder();
            XmlWriterSettings sett = new XmlWriterSettings();
            sett.ConformanceLevel = ConformanceLevel.Fragment;
            sett.Encoding = Encoding.GetEncoding("ISO-8859-1");
            using (XmlWriter w = XmlWriter.Create(sb, sett))
            {
                w.WriteString(unescaped);
                w.Flush();
                w.Close();
            }

            return sb.ToString();
        }

        /// <summary>
        /// Method that unescapes special characters in an XML string.
        /// </summary>
        /// <param name="escaped">The escaped XML string.</param>
        /// <returns>The unescaped string.</returns>
        static public String XmlUnescape(String escaped)
        {
            XmlDocument doc = new XmlDocument();
            var node = doc.CreateElement("root");
            node.InnerXml = escaped;
            return node.InnerText;
        }
    }

    /// <summary>
    /// Class providing Unicode information about characters in the range 0x0000-0xFFFF.
    /// </summary>
    class UnicodeInfo
    {
        /// <summary>
        /// Mapping from character (code points) to Unicode names.
        /// </summary>
        private SortedDictionary<char, String> uniNames;

        /// <summary>
        /// (Ordered) list of Unicode blocks (first character, last character, block name).
        /// </summary>
        private List<Tuple<uint, uint, String>> uniBlocks;

        /// <summary>
        /// Constructor, which takes the information from text files, as supplied by http://www.unicode.org, in the format specified at http://www.unicode.org/reports/tr44/#Format_Conventions.
        /// </summary>
        /// <param name="UnicodeCharData">Unicode character data for the range 0x0000-0xFFFF, given in a line per character, with fields delimited by semicolons, as specified at http://www.unicode.org/Public/UCD/latest/ucd/UnicodeData.txt .</param>
        /// <param name="UnicodeBlocks">Unicode block data for the range 0x0000-0xFFFF, as contained in http://www.unicode.org/Public/UCD/latest/ucd/Blocks.txt .</param>
        public UnicodeInfo(String[] UnicodeCharData, String[] UnicodeBlocks)
        {
            // initialize mapping of chars to names
            uniNames = new SortedDictionary<char, String>();
            int current_char;
            String current_description;
            // process all character codes
            int i = 0;
            while (i < UnicodeCharData.Length)
            {
                String[] fields = UnicodeCharData[i].Split(';');
                if (0 < fields.Length)
                {
                    // extract information from the fields
                    current_char = int.Parse(fields[0], NumberStyles.HexNumber);
                    current_description = fields[1];
                    // check if it's a range and if so, update the description and get the end of the range from the next character code description
                    if (current_description[0] != '<' || !current_description.EndsWith(", First>"))
                        // not a range, add the character and its description
                        uniNames.Add((char)current_char, current_description);
                    else
                    {
                        // it's (the start of) a range
                        // update description
                        current_description = current_description.Substring(1, current_description.Length - 9);
                        // get the end of the range from the next character code description
                        fields = UnicodeCharData[++i].Split(';');
                        int end_of_range = int.Parse(fields[0], NumberStyles.HexNumber);
                        if (!fields[1].EndsWith(", Last>"))
                            throw new Exception(String.Format("Character end of range expected (after {0:X4}).", current_char));
                        // add all characters in the range to the dictionary
                        for (int j = current_char; j <= end_of_range; j++)
                            uniNames.Add((char)j, current_description);
                    }
                }
                i++;
            }

            // extract block information
            uniBlocks = new List<Tuple<uint, uint, String>>();
            foreach (String s in UnicodeBlocks)
            {
                uint startRange = uint.Parse(s.Substring(0, 4), NumberStyles.HexNumber);
                uint endRange = uint.Parse(s.Substring(6, 4), NumberStyles.HexNumber);
                uniBlocks.Add(new Tuple<uint, uint, String>(startRange, endRange, s.Substring(12)));
            }
        }

        /// <summary>
        /// Function returning a character's Unicode name (or null if not found).
        /// </summary>
        /// <param name="c">A character.</param>
        /// <returns>The Unicode name for the character (or null if not found).</returns>
        public String getCharName(char c)
        {
            String s;
            if (uniNames.TryGetValue(c, out s))
                return s;
            else
                return null;
        }

        /// <summary>
        /// Function returning the name of the Unicode block to which a character belongs (or null if not found).
        /// </summary>
        /// <param name="c">A character.</param>
        /// <returns>The name of the Unicode block to which the character belongs (or null if not found).</returns>
        public String getCharBlock(char c)
        {
            if (0 < uniBlocks.Count)
            {
                // find block with binary search
                uint d = (uint)c;
                int x = 0;
                int y = uniBlocks.Count;

                // uniBlocks[x].Item1 <= d <= uniBlocks[y].Item1
                while (x + 1 != y)
                {
                    // 0 <= x < x + 1 < y <= uniBlocks.Count
                    // uniBlocks[x].Item1 <= d < uniBlocks[y].Item1
                    int z = (x + y) / 2;
                    if (uniBlocks[z].Item1 <= d)
                        x = z;
                    else
                        y = z;
                }

                // uniBlocks[x].Item1 <= d < uniBlocks[y].Item1 && x + 1 == y)
                if (d <= uniBlocks[x].Item2)
                    return uniBlocks[x].Item3;
                else
                    return null;
            }
            else
                return null;
        }
    }
}
