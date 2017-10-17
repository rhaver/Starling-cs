using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace StarlingDB
{

    class StarlingDecoder
    {
        #region Markers

        /// <summary>
        /// Byte value that marks the onset of double byte mode.
        /// </summary>
        private const byte double_byte_mode_startmark = 0x01;

        /// <summary>
        /// Byte value that marks the end of double byte mode.
        /// </summary>
        private const byte double_byte_mode_endmark = 0x7f;

        /// <summary>
        /// Byte value that marks the next character as special (in single byte mode).
        /// </summary>
        private const byte special_char_next_mark = 0x1d;

        #endregion

        #region Other constants

        /// <summary>
        /// The placeholder character to display where decoding failed.
        /// </summary>
        public const String fallbackCharacter = "\u2620"; // '☠': skull and crossbones (alternatively, use the alert character "\a"?)

        /// <summary>
        /// Indicates whether characters from the Big5 code space are accepted (true), or are treated as anomalous (false).
        /// </summary>
        public const bool useBig5 = false;

        /// <summary>
        /// The normalization form in which to return decoded Unicode strings (NFC).
        /// </summary>
        public const NormalizationForm normalizationForm = NormalizationForm.FormC;

        #endregion

        #region Character tables/references

        /// <summary>
        /// The various combining accents that are encoded in a single byte.
        /// </summary>
        public readonly static SortedDictionary<byte, char> sbCombiningAccents = new SortedDictionary<byte, char> 
        {
            { 0x5e, (char)0x302 }, { 0x7e, (char)0x303 },
            { 0xb1, (char)0x301 }, { 0xb3, (char)0x328 }, { 0xbb, (char)0x307 }, { 0xbc, (char)0x308 }, { 0xbf, (char)0x32f },
            { 0xc4, (char)0x304 }, { 0xc8, (char)0x30a }, { 0xc9, (char)0x325 },
            { 0xdb, (char)0x300 }, { 0xdc, (char)0x323 }, { 0xdf, (char)0x306 },
            { 0xf6, (char)0x30c }
        };
        
        /// <summary>
        /// Replacement values for ASCII control characters encountered in single byte mode, that represent layout informatino.
        /// </summary>
        public readonly static SortedDictionary<byte, String> sbAsciiControlCharLayout = new SortedDictionary<byte, String>
        {
            { 0x09, "\t" }, { 0x0a, "\n" }, { 0x0d, null }, { 0x15, "\n\n" }
            // alternatively, consider U+2028, U+2029
        };

        /// <summary>
        /// Single byte mode characters that do not fall in the ASCII or CP866 encoding.
        /// </summary>
        public readonly static SortedDictionary<byte, String> sbNonCP866 = new SortedDictionary<byte, String>
        {
            { 0xb0, "ā" }, { 0xb1, "\u0301" }, { 0xb2, "ä" }, { 0xb3, "\u0328" }, { 0xb4, "ǟ" }, { 0xb5, "c̣" }, { 0xb6, "č" }, { 0xb7, "č̣" }, { 0xb8, "δ" }, { 0xb9, "ē" }, { 0xba, fallbackCharacter }, { 0xbb, "\u0307" }, { 0xbc, "\u0308" }, { 0xbd, "ɛ" }, { 0xbe, "ʡ" }, { 0xbf, "\u032f" },
            { 0xc0, "ç" }, { 0xc1, "ɣ" }, { 0xc2, "ʁ" }, { 0xc3, "ħ" }, { 0xc4, "\u0304" }, { 0xc5, "ī" }, { 0xc6, "ɨ" }, { 0xc7, "ɨ̄" }, { 0xc8, "\u030a" }, { 0xc9, "\u0325" }, { 0xca, "ḳ" }, { 0xcb, "ʎ" }, { 0xcc, "ƛ" }, { 0xcd, "-" }, { 0xce, "ƛ̣" }, { 0xcf, "ɫ" },
            { 0xd0, "Ɫ" }, { 0xd1, "ŋ" }, { 0xd2, "ō" }, { 0xd3, "ö" }, { 0xd4, "ȫ" }, { 0xd5, "ɔ" }, { 0xd6, "ɔ̄" }, { 0xd7, "ṗ" }, { 0xd8, "q̇" }, { 0xd9, "ß" }, { 0xda, "~" }, { 0xdb, "\u0300" }, { 0xdc, "\u0323" }, { 0xdd, "š" }, { 0xde, "ṭ" }, { 0xdf, "\u0306" },
            { 0xf0, "ϑ" }, { 0xf1, "ū" }, { 0xf2, "ü" }, { 0xf3, "ǖ" }, { 0xf4, "ə" }, { 0xf5, "ə̄" }, { 0xf6, "\u030c" }, { 0xf7, "ʷ" }, { 0xf8, "ɦ" }, { 0xf9, "χ"}, { 0xfa, "ʒ" }, { 0xfb, "ǯ" }, { 0xfc, "ž" }, { 0xfd, "ʔ" }, { 0xfe, "ʕ" }, { 0xff, "ʌ" }
        };

        /// <summary>
        /// Characters represented by bytes that are marked as 'special' in single byte mode.
        /// </summary>
        public readonly static SortedDictionary<byte, char> sbSpecialChars = new SortedDictionary<byte, char>
        {
            { 0x4c, 'ɬ' }, { 0x61, 'æ' }, { 0x62, 'ƀ' }, { 0x63, 'ɕ' }, { 0x64, 'ð' }, { 0x65, 'œ' }, { 0x67, 'ǥ' }, { 0x68, 'ḫ' },
            { 0x69, 'ı' }, { 0x6a, 'ȷ' }, { 0x6f, 'ø' }, { 0x72, 'ɾ' }, { 0x73, 'ʃ' }, { 0x74, 'þ' }, { 0x78, 'ᶚ' }, { 0x41, 'Ӕ' },
            { 0x44, 'đ' }, { 0x54, 'ŧ' },
            { 0x5e, (char)0x311 },
            { 0xa3, 'ɠ' }, { 0xa5, 'ѧ' }, { 0xa7, 'ѕ' }, { 0xab, 'љ' }, { 0xad, 'њ' }, { 0xae, 'ѫ' },
            { 0xb1, (char)0x30b }, { 0xbf, (char)0x32e }, { 0xdb, (char)0x30f },
            { 0xe0, 'ʁ' }, { 0xed, 'є' }, { 0xef, 'ž' }, { 0xf8, 'ƕ' }, { 0xfa, 'ђ' }
        };

        /// <summary>
        /// Mapping for the (Greek) characters encoded by the second byte in double byte mode, when the first byte is 0x83.
        /// </summary>
        public readonly static SortedDictionary<byte, String> dbGreek_83 = new SortedDictionary<byte, String>
        {
            { 0x90, "\u0344" }, { 0x91, "\u0314\u0301" }, { 0x92, "\u0313\u0301" }, { 0x93, "\u0301" }, { 0x9a, "\u0308\u0300" }, { 0x9b, "\u0314\u0300" }, { 0x9c, "\u0313\u0300" }, { 0x9d, "\u0314\u0342" }, { 0x9e, "·" },
            { 0xa0, "\u0314" }, { 0xa1, "\u0313" }, { 0xa3, "\u0308" }, { 0xa4, "Α" }, { 0xa5, "Β" }, { 0xa6, "Χ" }, { 0xa7, "Δ" }, { 0xa8, "Ε" }, { 0xa9, "Φ" }, { 0xaa, "Γ" }, { 0xab, "Η" }, { 0xac, "Ι" }, { 0xad, "ῳ" }, { 0xae, "Κ" }, { 0xaf, "Λ" },
            { 0xb0, "Μ" }, { 0xb1, "Ν" }, { 0xb2, "Ο" }, { 0xb3, "Π" }, { 0xb4, "Θ" }, { 0xb5, "Ρ" }, { 0xb6, "Σ" }, { 0xb7, "Τ" }, { 0xb8, "Υ" }, { 0xb9, "ῃ" }, { 0xba, "Ω" }, { 0xbb, "Ξ" }, { 0xbc, "Ψ" }, { 0xbd, "Ζ" },
            { 0xc0, "\u0313\u0342" }, { 0xc1, "\u0300" }, { 0xc2, "α" }, { 0xc3, "β" }, { 0xc4, "χ" }, { 0xc5, "δ" }, { 0xc6, "ε" }, { 0xc7, "φ" }, { 0xc8, "γ" }, { 0xc9, "η" }, { 0xca, "ι" }, { 0xcb, "ς" }, { 0xcc, "κ" }, { 0xcd, "λ" }, { 0xce, "μ" }, { 0xcf, "ν" },
            { 0xd0, "ο" }, { 0xd1, "π" }, { 0xd2, "θ" }, { 0xd3, "ρ" }, { 0xd4, "σ" }, { 0xd5, "τ" }, { 0xd6, "υ" }, { 0xd7, "ᾳ" }, { 0xd8, "ω" }, { 0xd9, "ξ" }, { 0xda, "ψ" }, { 0xdb, "ζ" }, { 0xdc, "\u0342" }
        };

        /// <summary>
        /// Mapping for the (Greek) characters encoded by the second byte in double byte mode, when the first byte is 0x85.
        /// </summary>
        public readonly static SortedDictionary<byte, String> dbGreek_85 = new SortedDictionary<byte, String>
        {
            { 0xaf, "ϝ" }
        };

        /// <summary>
        /// Mapping for the (OCS) characters encoded by the second byte in double byte mode, when the first byte is 0x87.
        /// </summary>
        public readonly static SortedDictionary<byte, String> dbOCS_87 = new SortedDictionary<byte, String>
        {
            { 0x83, "\u0433\u0311" }, { 0x84, "б" }, { 0x86, "ю" },
            { 0x93, "ж" }, { 0x96, "Ю" }, { 0x9a, "И" }, { 0x9b, "С" }, { 0x9e, "А" }, { 0x9f, "П" },
            { 0xa0, "Р" }, { 0xa2, "О" }, { 0xa3, "Л" }, { 0xa4, "Д" }, { 0xa8, "З" }, { 0xaa, "К" }, { 0xac, "Е" }, { 0xad, "Г" }, { 0xae, "М" },
            { 0xb1, "Н" }, { 0xb3, "х" }, { 0xb8, "ъ" }, { 0xb9, "ф" }, { 0xba, "и" }, { 0xbb, "с" }, { 0xbc, "в" }, { 0xbd, "у" }, { 0xbe, "а" }, { 0xbf, "п" },
            { 0xc0, "р" }, { 0xc1, "ш" }, { 0xc2, "о" }, { 0xc3, "л" }, { 0xc4, "д" }, { 0xc5, "ь" }, { 0xc6, "т" }, { 0xc8, "з" }, { 0xca, "к" }, { 0xcb, "ы" }, { 0xcc, "е" }, { 0xcd, "г" }, { 0xce, "м" }, { 0xcf, "ц" },
            { 0xd0, "ч" }, { 0xd1, "н" }, { 0xd2, "я" }, { 0xd3, "Х" }, { 0xd5, "ѣ" }, { 0xd8, "\ua657" }, { 0xda, "ѧ" },
            { 0xe7, "ѫ" }, { 0xe8, "Е" }, { 0xe9, "e" },
            { 0xf2, "ѡ" }, { 0xf3, "Ѭ" }, { 0xf4, "ѭ" }, { 0xf5, "Ѥ" }, { 0xf6, "ѥ" }
        };

        /// <summary>
        /// Mapping for the (OCS) characters encoded by the second byte in double byte mode, when the first byte is 0x88.
        /// </summary>
        public readonly static SortedDictionary<byte, String> dbOCS_88 = new SortedDictionary<byte, String>
        {
            { 0x81, "ѵ" }, { 0x83, "ѧ" }
        };

        /// <summary>
        /// Mapping for custom encoded ranges in double byte mode, based on the first byte.
        /// </summary>
        private static SortedDictionary<byte, SortedDictionary<byte, String>> doubleByteTableRef = new SortedDictionary<byte, SortedDictionary<byte, String>>
        {
            { 0x83, dbGreek_83 }, { 0x85, dbGreek_85 }, { 0x87, dbOCS_87 }, { 0x88, dbOCS_88 }
        };

        #endregion

        #region Encoding objects

        private static Encoding CP866_encoder = Encoding.GetEncoding(866);
        private static Encoding Big5 = Encoding.GetEncoding(950);

        #endregion

        #region Constructors

        /// <summary>
        /// Static constructor. Automatically gets called once before any static method is used.
        /// </summary>
        static StarlingDecoder() { }

        #endregion

        #region Decoding functions

        /// <summary>
        /// Function to decode a character represented by a single byte. Assumed is that combining characters (in the ASCII range) do not turn up here.
        /// </summary>
        /// <param name="b">The byte to be decoded.</param>
        /// <returns>The character encoded by the byte or the placeholder in case failure.</returns>
        private static String decodeSingleByte(byte b)
        {
            String s;
            if (b < 0x20)
            {
                // byte is in the range of ASCII that consists of control characters
                if (sbAsciiControlCharLayout.ContainsKey(b))
                    s = sbAsciiControlCharLayout[b];
                else
                {
                    // we're not expecting a control character here
                    Trace.WriteLine("Unexpected ASCII control character: 0x" + b.ToString("X2"));
                    s = fallbackCharacter;
                }
            }
            else if (b < 0x7f)
                // we are dealing with a normal ASCII char
                s = new String((char)b, 1);
            else if (b == 0x7f)
                // DEL character, ignore
                s = null;
            else if (b <= 0xaf)
                // we are in the range of the CP866 encoding (0x80-0xaf), that can be transposed to Unicode by adding 0x390
                s = new String((char)(b + 0x390), 1);
            else if (b <= 0xdf)
                // we are outside the range of ASCII/CP866, so we look up the encoded character in a lookup table
                s = sbNonCP866[b];
            else if (b <= 0xef)
                // we are in the range of the CP866 encoding (0xe0-0xef), that can be transposed to Unicode by adding 0x360
                s = new String((char)(b + 0x360), 1);
            else
                // we are outside the range of ASCII/CP866, so we look up the encoded character in a lookup table
                s = sbNonCP866[b];
            return s;
        }

        /// <summary>
        /// Function to decode a characters represented by a byte that is marked as 'special' in single byte mode.
        /// </summary>
        /// <param name="b">The 'special' byte to be decoded.</param>
        /// <returns>The character encoded by the special byte or the placeholder in case of failure.</returns>
        private static String decodeSpecialSingleByte(byte b)
        {
            String s;
            if (sbSpecialChars.ContainsKey(b))
                s = sbSpecialChars[b].ToString();
            else
            {
                Trace.WriteLine("This is not a (known) special char: 0x" + b.ToString("X2"));
                s = fallbackCharacter;
            }
            return s;
        }

        /// <summary>
        /// Function to decode a character represented by two bytes.
        /// </summary>
        /// <param name="b1">The first byte of the character to be decoded.</param>
        /// <param name="b2">The first byte of the character to be decoded.</param>
        /// <returns>The character encoded by the bytes or the placeholder in case failure.</returns>
        private static String decodeDoubleByte(byte b1, byte b2)
        {
            String s;
            // check if, based on the first byte, we have a lookup table for the second byte
            if (doubleByteTableRef.ContainsKey(b1))
            {
                SortedDictionary<byte, string> lookuptable = doubleByteTableRef[b1];
                // check if the lookup table contains an entry for the second byte
                if (lookuptable.ContainsKey(b2))
                    s = lookuptable[b2];
                else
                {
                    s = fallbackCharacter;
                    Trace.WriteLine("Unknown double byte sequence: " + BitConverter.ToString(new byte[] { b1, b2 }));
                }
            }
            else
            {
                // we do not have our own lookup table for this range, so treat is as a character encoded in Big5, or give warning
                if (useBig5)
                    s = Encoding.Unicode.GetString(Encoding.Convert(Big5, Encoding.Unicode, new byte[] { b1, b2 }));
                else
                {
                    Trace.WriteLine("Big5 character");
                    s = fallbackCharacter;
                }
            }
            return s;
        }

        /// <summary>
        /// Function to decode text stored in a byte array in Starling encoding.
        /// </summary>
        /// <param name="ba">The byte array to be decoded.</param>
        /// <returns>The decoded text in <value>normalizationForm</value>.</returns>
        public static String decode(byte[] ba)
        {
            StringBuilder sb = new StringBuilder();
            bool double_byte_mode = false;

            // walk through all bytes in the array
            int i = 0;
            while (i < ba.Length)
            {
                if (sbCombiningAccents.ContainsKey(ba[i]))
                {
                    // combining accents can occur in both single and double byte mode
                    sb.Append(sbCombiningAccents[ba[i]]);
                    i++;
                }
                else
                {
                    // we are not dealing with a combining accent, so let's see what we are dealing with (depending on which byte mode we're in)
                    if (!double_byte_mode)
                    {
                        // we are in single byte mode
                        if (ba[i] == special_char_next_mark)
                        {
                            // next char will be 'special'
                            if (i + 1 < ba.Length) // boundary check
                                sb.Append(decodeSpecialSingleByte(ba[i + 1]));
                            else
                                Trace.WriteLine("Incomplete special character sequence.");
                            i += 2;
                        }
                        else
                        {
                            if (ba[i] == double_byte_mode_startmark)
                                // entering double byte mode
                                double_byte_mode = true;
                            else
                                // normal single byte mode
                                sb.Append(decodeSingleByte(ba[i]));
                            i++;
                        }
                    }
                    else
                    {
                        // we are in double byte mode
                        if (ba[i] == double_byte_mode_startmark)
                            // this may be a repeat of the double byte mode marker after a combining accent
                            i++;
                        else if (ba[i] <= 0x7f)
                            // an ASCII character signals a return to single byte mode
                            double_byte_mode = false;
                        else
                        {
                            // decode double byte
                            if (i + 1 < ba.Length) // boundary check
                                sb.Append(decodeDoubleByte(ba[i], ba[i + 1]));
                            else
                                Trace.WriteLine("Incomplete 2-byte sequence.");
                            i += 2;
                        }
                    }
                }
            }
            return sb.ToString().Normalize(normalizationForm);
        }

        #endregion

        #region XeLaTeX generating functions

        /// <summary>
        /// Generate XeLaTeX code for displaying a decoded unicode character (sequence).
        /// </summary>
        /// <param name="s">The decoded unicode character (sequence).</param>
        /// <returns>XeLaTeX code for displaying the unicode character (sequence).</returns>
        private static String generateXeLaTeXDecodedSample(String s)
        {
            bool firstchar = true;
            bool iscombining = false;
            StringBuilder sb = new StringBuilder();
            foreach (char c in s)
            {
                UInt16 i = (UInt16)c;
                if (firstchar)
                {
                    // check if the first character is a combining character
                    if ((0x0300 <= i && i <= 0x036F)
                        || (0x1DC0 <= i && i <= 0x1DFF)
                        || (0x20D0 <= i && i <= 0x20FF)
                        || (0xFE20 <= i && i <= 0xFE2F))
                    {
                        // wrap everything in a macro for dealing with combining characters
                        iscombining = true;
                        sb.Append(@"\combining{");
                    }
                    firstchar = false;
                }
                // add XeLaTeX code for displaying this Unicode character
                sb.Append(@"\char""");
                sb.Append(i.ToString("X4"));
            }
            // if it was a combining character (sequence), close the macro that wraps it
            if (iscombining)
                sb.Append(@"}");
            return sb.ToString();
        }

        /// <summary>
        /// Get a text representation showing the sequence of unicode values making up the input string.
        /// </summary>
        /// <param name="s">The string to be represented.</param>
        /// <param name="separator">A separator to put in between the representations for each character in the input string.</param>
        /// <returns>Text representation showing the sequence of unicode values (in hexadecimal notation).</returns>
        public static String stringToStringOfUnicodeValues(String s, char? separator = ' ')
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in s)
            {
                UInt16 i = (UInt16)c;
                //sb.Append("U+");
                sb.Append(i.ToString("X4"));
                sb.Append(separator);
            }
            if (separator != null)
                sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }
        
        /// <summary>
        /// Generate XeLaTeX code for a tabular's content, containing byte values in a certain range and their corresponding encoded character as given in a lookuptable.
        /// </summary>
        /// <param name="lookuptable">The lookuptable.</param>
        /// <param name="start">Start of the byte range.</param>
        /// <param name="end">End of the byte range.</param>
        /// <param name="rowSeparator">Code for the row separator.</param>
        /// <param name="rowPrepend">Code for the column separator.</param>
        /// <param name="prefixRowIndex">Whether or not to show the index at the start of each row.</param>
        /// <returns>XeLaTeX code for the data in a table to display character codes.</returns>
        private static String generateXeLaTeXTabularBody(SortedDictionary<byte, String> lookuptable, byte start, byte end, String rowSeparator = "", String rowPrepend = "", bool prefixRowIndex = true)
        {
            StringBuilder sb = new StringBuilder();
            for (uint i = start; i <= end; i++)
            {
                if (i % 16 == 0)
                {
                    sb.Append(rowPrepend);
                    if (prefixRowIndex)
                    {
                        sb.Append(@"\texttt{");
                        sb.Append((((byte)i) >> 4).ToString("X"));
                        sb.Append(@"\_} & ");
                    }
                }

                if (lookuptable.ContainsKey((byte)i))
                    sb.Append(String.Format(@"\unichar{{{0}}}{{{1}}}{{{2}}}{{{3}}}", i, i.ToString("X2"), generateXeLaTeXDecodedSample(lookuptable[(byte)i]), stringToStringOfUnicodeValues(lookuptable[(byte)i], ' ')));
                else
                    sb.Append(@"\missingchar{}");

                if (i % 16 == 15)
                {
                    sb.Append("\\\\");
                    if (i != end)
                        sb.AppendLine("\n" + rowSeparator);
                }
                else
                    sb.Append(" & ");
            }
            return sb.ToString();
        }

        /// <summary>
        /// Generate XeLaTeX code for starting a table to display character codes.
        /// </summary>
        /// <param name="label">A label for XeLaTeX to store the maximum width of any column in this table.</param>
        /// <param name="rowSeparator">Code for the row separator.</param>
        /// <param name="colSeparator">Code for the column separator.</param>
        /// <returns>XeLaTeX code for starting a table to display character codes.</returns>
        private static String generateXeLaTeXTableBegin(String label, String rowSeparator = "", String colSeparator = "")
        {
            StringBuilder sb = new StringBuilder("\\begin{charactertable}\n\\begin{tabular}{r");
            String col = colSeparator + @"C{" + label + "}";
            for (int i = 0; i < 16; i++)
                sb.Append(col);
            sb.Append("}\n\\hline\n &\\texttt{\\_0}&\\texttt{\\_1}&\\texttt{\\_2}&\\texttt{\\_3}&\\texttt{\\_4}&\\texttt{\\_5}&\\texttt{\\_6}&\\texttt{\\_7}&\\texttt{\\_8}&\\texttt{\\_9}&\\texttt{\\_A}&\\texttt{\\_B}&\\texttt{\\_C}&\\texttt{\\_D}&\\texttt{\\_E}&\\texttt{\\_F}\\\\\n");
            sb.Append(rowSeparator);
            return sb.ToString();
        }

        /// <summary>
        /// Generate a XeLaTeX document containing tables outlining the character encoding.
        /// </summary>
        /// <returns></returns>
        public static String generateXeLaTeX()
        {
            String preamble = @"%!TEX TS-program = xelatex
%!TEX encoding = UTF-8 Unicode

\documentclass[12pt]{article}

\usepackage{fontspec}
\usepackage{xunicode,xltxtra}

\setmainfont{DejaVu Serif}
\newfontfamily\EncodingFont{DejaVu Sans}

\usepackage[table]{xcolor}
\usepackage{pdflscape}
\usepackage{eqparbox}

\newcommand{\unichar}[4]{\begin{tabular}[c]{@{}c@{}c}\\[-.9em]\EncodingFont#3\\{\tiny#4}\end{tabular}}
\newcommand{\missingchar}{\phantom{\unichar{65}{41}{A}{0041}}}
\newcommand{\combining}[1]{\color{combining}\char""25CC#1}
\newenvironment{charactertable}{\begin{landscape}\begin{table}\begin{center}\footnotesize}{\end{center}\end{table}\end{landscape}}

\newsavebox{\tstretchbox}
\newcolumntype{C}[1]{>{\begin{lrbox}{\tstretchbox}}l<{\end{lrbox}\eqmakebox[#1][c]{\unhcopy\tstretchbox}}}

\definecolor{ascii}{RGB}{255,221,221}
\definecolor{cp866}{RGB}{221,255,221}
\definecolor{sbstarling}{RGB}{221,221,255}
\definecolor{combining}{RGB}{255,0,0}

\begin{document}";
            String tableEnd = "\\hline\n\\end{tabular}\n\\end{charactertable}\n\n";
            String rowSeparator = @"\hline";

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(preamble);

            // entire single byte range
            sb.AppendLine(generateXeLaTeXTableBegin("sb", rowSeparator, ""));
            SortedDictionary<byte, String> sbCP866 = new SortedDictionary<byte, String>();
            for (uint i = 0x20; i <= 0xff; i++)
                sbCP866.Add((byte)i, CP866_encoder.GetString(new byte[] { (byte)i }));
            foreach (byte b in sbCombiningAccents.Keys)
                if (sbCP866.ContainsKey(b))
                    sbCP866[b] = sbCombiningAccents[b].ToString();
            sbCP866.Remove(0x7f);
            bool slkdfj = sbCP866.ContainsKey(0xba);
            sb.AppendLine(generateXeLaTeXTabularBody(sbCP866, 0x20, 0x7f, rowSeparator, @"\rowcolor{ascii}"));
            sb.AppendLine(rowSeparator);
            sb.AppendLine(generateXeLaTeXTabularBody(sbCP866, 0x80, 0xaf, rowSeparator, @"\rowcolor{cp866}"));
            sb.AppendLine(rowSeparator);
            sbNonCP866.Remove(0xba);
            sb.AppendLine(generateXeLaTeXTabularBody(sbNonCP866, 0xb0, 0xdf, rowSeparator, @"\rowcolor{sbstarling}"));
            sb.AppendLine(rowSeparator);
            sb.AppendLine(generateXeLaTeXTabularBody(sbCP866, 0xe0, 0xef, rowSeparator, @"\rowcolor{cp866}"));
            sb.AppendLine(rowSeparator);
            sb.AppendLine(generateXeLaTeXTabularBody(sbNonCP866, 0xf0, 0xff, rowSeparator, @"\rowcolor{sbstarling}"));
            sb.AppendLine(tableEnd);

            // single byte special characters
            sb.AppendLine(generateXeLaTeXTableBegin("sbs", rowSeparator, ""));
            SortedDictionary<byte, String> sbSpecialCharsString = new SortedDictionary<byte, String>();
            foreach (KeyValuePair<byte, char> kv in sbSpecialChars)
                sbSpecialCharsString.Add(kv.Key, kv.Value.ToString());
            sb.AppendLine(generateXeLaTeXTabularBody(sbSpecialCharsString, 0x40, 0xff, rowSeparator));
            sb.AppendLine(tableEnd);

            // greek
            sb.AppendLine(generateXeLaTeXTableBegin("gr1", rowSeparator, ""));
            sb.AppendLine(generateXeLaTeXTabularBody(dbGreek_83, 0x90, 0xdf, rowSeparator));
            sb.AppendLine(tableEnd);

            sb.AppendLine(generateXeLaTeXTableBegin("gr2", rowSeparator, "@{}"));
            sb.AppendLine(generateXeLaTeXTabularBody(dbGreek_85, 0xa0, 0xaf, rowSeparator));
            sb.AppendLine(tableEnd);

            // ocs
            sb.AppendLine(generateXeLaTeXTableBegin("ocs1", rowSeparator, @""));
            sb.AppendLine(generateXeLaTeXTabularBody(dbOCS_87, 0x80, 0xff, rowSeparator));
            sb.AppendLine(tableEnd);

            sb.AppendLine(generateXeLaTeXTableBegin("ocs2", rowSeparator, @"@{}"));
            sb.AppendLine(generateXeLaTeXTabularBody(dbOCS_88, 0x80, 0x8f, rowSeparator));
            sb.AppendLine(tableEnd);

            sb.AppendLine(@"\end{document}");

            return sb.ToString();
        }

        #endregion

    }
}