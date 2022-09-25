using System;
using System.Collections.Generic;
using System.Text;

namespace Fce.Utils
{
    internal class StringLengthEncode
    {
        /// <summary>
        /// Replace recurring characters with a single character with a special character prefix denoting quanity.
        /// Used to try and shorten a string.
        /// </summary>
        /// <param name="inputString">The string to short</param>
        /// <returns>Shortened string</returns>
        internal static string Encode(string inputString)
        {
            var buffer = new StringBuilder();

            if (!string.IsNullOrEmpty(inputString))
            {
                var queue = new Queue<char>(inputString);
                int count = 0;

                while (queue.Count > 0)
                {
                    char character = queue.Dequeue();
                    count++;

                    if (queue.Count == 0 || character != queue.Peek())
                    {
                        if (count > 1)
                        {
                            if (count > 30)
                            {
                                buffer.Append("ð");
                                buffer.Append(count);
                            }
                            else
                                buffer.Append(ConvertToBase30Numeric(count));
                        }
                        buffer.Append(character);

                        if (count > 30)
                            buffer.Append("ç");

                        count = 0;
                    }
                }
            }

            return buffer.ToString();
        }

        /// <summary>
        /// Unshorten a string - Replace special characters denoting quanity of recurring characters with actual characters.
        /// </summary>
        /// <param name="inputString">String to un-shorten</param>
        /// <returns>Original string (before length encode)</returns>
        internal static string Decode(string inputString)
        {
            inputString = ReplaceStringWithStartAndEndEncoding(inputString);

            int start = inputString.IndexOf("ð");
            int end = inputString.IndexOf("ç");

            string textBefore = inputString.Substring(0, start);
            string textAfter = inputString.Substring(end + 1);
            string between = inputString.Substring(start + 1, end - start - 1);

            int count = Convert.ToInt32(between.Substring(0, between.Length - 1));
            char replaceChar = between[between.Length - 1];
            string replaceText = string.Empty;

            for (int i = 0; i < count; i++)
                replaceText += replaceChar;

            string replacementText = textBefore + replaceText + textAfter;

            if (replacementText.Contains("ð"))
                return Decode(replacementText);
            else
                return replacementText;
        }

        /// <summary>
        /// Unshorten a string subroutine - Replace special characters denoting quanity of recurring characters with actual characters.
        /// </summary>
        /// <param name="inputString">String to un-shorten</param>
        /// <returns>Original string (before length encode)</returns>
        private static string ReplaceStringWithStartAndEndEncoding(string inputString)
        {
            inputString = ReplaceStringWithStartAndEndEncoding(inputString, "~");
            inputString = ReplaceStringWithStartAndEndEncoding(inputString, "Ʉ");
            inputString = ReplaceStringWithStartAndEndEncoding(inputString, "ã");
            inputString = ReplaceStringWithStartAndEndEncoding(inputString, "ƒ");
            inputString = ReplaceStringWithStartAndEndEncoding(inputString, "µ");
            inputString = ReplaceStringWithStartAndEndEncoding(inputString, "á");
            inputString = ReplaceStringWithStartAndEndEncoding(inputString, "ÿ");
            inputString = ReplaceStringWithStartAndEndEncoding(inputString, "ü");
            inputString = ReplaceStringWithStartAndEndEncoding(inputString, "Ó");
            inputString = ReplaceStringWithStartAndEndEncoding(inputString, "Ã");
            inputString = ReplaceStringWithStartAndEndEncoding(inputString, "Þ");
            inputString = ReplaceStringWithStartAndEndEncoding(inputString, "¤");
            inputString = ReplaceStringWithStartAndEndEncoding(inputString, "»");
            inputString = ReplaceStringWithStartAndEndEncoding(inputString, "§");
            inputString = ReplaceStringWithStartAndEndEncoding(inputString, "é");
            inputString = ReplaceStringWithStartAndEndEncoding(inputString, "ë");
            inputString = ReplaceStringWithStartAndEndEncoding(inputString, "å");
            inputString = ReplaceStringWithStartAndEndEncoding(inputString, "ì");
            inputString = ReplaceStringWithStartAndEndEncoding(inputString, "Ú");
            inputString = ReplaceStringWithStartAndEndEncoding(inputString, "Ï");
            inputString = ReplaceStringWithStartAndEndEncoding(inputString, "Å");
            inputString = ReplaceStringWithStartAndEndEncoding(inputString, "¦");
            inputString = ReplaceStringWithStartAndEndEncoding(inputString, "Ñ");
            inputString = ReplaceStringWithStartAndEndEncoding(inputString, "†");
            inputString = ReplaceStringWithStartAndEndEncoding(inputString, "ˆ");
            inputString = ReplaceStringWithStartAndEndEncoding(inputString, "ï");
            inputString = ReplaceStringWithStartAndEndEncoding(inputString, "ȑ");
            inputString = ReplaceStringWithStartAndEndEncoding(inputString, "Â");
            inputString = ReplaceStringWithStartAndEndEncoding(inputString, "Ò");
            inputString = ReplaceStringWithStartAndEndEncoding(inputString, "Ý");

            if (inputString.StringContainsIn(
                "~", "Ʉ", "ã", "ƒ", "µ", "á", "ÿ", "ü", "Ó", "Ã", "Þ", "¤", "»", "§", "é",
                "ë", "å", "ì", "Ú", "Ï", "Å", "¦", "Ñ", "†", "ˆ", "ï", "ȑ", "Â", "Ò", "Ý"))
            {
                return ReplaceStringWithStartAndEndEncoding(inputString);
            }

            return inputString;
        }

        /// <summary>
        /// Unshorten a string subroutine - Replace special characters denoting quanity of recurring characters with actual characters - 
        /// for a specific character.
        /// </summary>
        /// <param name="inputString">String to un-shorten</param>
        /// <param name="inputString">Specific special character to remove</param>
        /// <returns>Original string (before length encode)</returns>
        private static string ReplaceStringWithStartAndEndEncoding(string inputString, string specialCharacter)
        {
            if (inputString.Contains(specialCharacter))
            {
                int start = inputString.IndexOf(specialCharacter);
                int end = inputString.IndexOf(specialCharacter) + 1;

                string textBefore = inputString.Substring(0, start);
                string textAfter = inputString.Substring(end + 1);
                string between = inputString.Substring(start, end - start + 1);

                string countSc = between.Substring(0, between.Length - 1);
                char character = between[between.Length - 1];
                int count = ConvertFromBase30Numeric(countSc);

                return textBefore + "ð" + count + character + "ç" + textAfter;
            }

            return inputString;
        }

        /// <summary>
        /// Given a number, output a special character representing that number. Max 30
        /// </summary>
        /// <param name="number">Number to convert</param>
        /// <returns>Special character</returns>
        /// <exception cref="ApplicationException">Number must be between 1 and 30</exception>
        private static string ConvertToBase30Numeric(int number)
        {
            switch (number)
            {
                case 1: return "~";
                case 2: return "Ʉ";
                case 3: return "ã";
                case 4: return "ƒ";
                case 5: return "µ";
                case 6: return "á";
                case 7: return "ÿ";
                case 8: return "ü";
                case 9: return "Ó";
                case 10: return "Ã";
                case 11: return "Þ";
                case 12: return "¤";
                case 13: return "»";
                case 14: return "§";
                case 15: return "é";
                case 16: return "ë";
                case 17: return "å";
                case 18: return "ì";
                case 19: return "Ú";
                case 20: return "Ï";
                case 21: return "Å";
                case 22: return "¦";
                case 23: return "Ñ";
                case 24: return "†";
                case 25: return "ˆ";
                case 26: return "ï";
                case 27: return "ȑ";
                case 28: return "Â";
                case 29: return "Ò";
                case 30: return "Ý";
            }

            throw new ApplicationException("Special character map out of range -  You should never get here");
        }

        /// <summary>
        /// Given a special character, output the number it represents
        /// </summary>
        /// <param name="symbol">Special character representing a number</param>
        /// <returns>The number it represents</returns>
        /// <exception cref="ApplicationException">If character invalid (not mapped)</exception>
        private static int ConvertFromBase30Numeric(string symbol)
        {
            switch (symbol)
            {
                case "~": return 1;
                case "Ʉ": return 2;
                case "ã": return 3;
                case "ƒ": return 4;
                case "µ": return 5;
                case "á": return 6;
                case "ÿ": return 7;
                case "ü": return 8;
                case "Ó": return 9;
                case "Ã": return 10;
                case "Þ": return 11;
                case "¤": return 12;
                case "»": return 13;
                case "§": return 14;
                case "é": return 15;
                case "ë": return 16;
                case "å": return 17;
                case "ì": return 18;
                case "Ú": return 19;
                case "Ï": return 20;
                case "Å": return 21;
                case "¦": return 22;
                case "Ñ": return 23;
                case "†": return 24;
                case "ˆ": return 25;
                case "ï": return 26;
                case "ȑ": return 27;
                case "Â": return 28;
                case "Ò": return 29;
                case "Ý": return 30;
            }

            throw new ApplicationException("Special character map out of range - You should never get here");
        }
    }
}
