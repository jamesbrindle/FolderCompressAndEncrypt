using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Fce.Utils
{
    /// <summary>
    /// Methods for encrypting and decrypting strings, it also try to compress the strings so the result is shortened as much as possible.
    /// Handy when dealing with paths since an encrypted string is always longer.
    /// </summary>
    internal static class StringEncrypt
    {
        /// <summary>
        /// Encrypt a string. It it also try to compress the strings so the result is shortened as much as possible.
        /// Handy when dealing with paths since an encrypted string is always longer.
        /// </summary>
        /// <param name="clearText">The string to encrypt</param>
        /// <returns>The encrypted string</returns>
        internal static string Encrypt(string clearText)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(clearText);
            using (var memoryStream = new MemoryStream())
            {
                using (var gzipStream = new GZipStream(memoryStream, CompressionLevel.Optimal))
                    gzipStream.Write(bytes, 0, bytes.Length);

                string encrypted = "Š" + Convert.ToBase64String(memoryStream.ToArray()).Replace("/", "-").Replace("\\", "_");

                return StringLengthEncode.Encode(encrypted);
            }
        }

        /// <summary>
        /// Decrypts a string.
        /// </summary>
        /// <param name="cipherText">The string to decrypt</param>
        /// <returns>The decrypted string</returns>
        internal static string Decrypt(string cipherText)
        {
            cipherText = cipherText.Replace("Š", "");
            cipherText = cipherText.Replace("-", "/");
            cipherText = cipherText.Replace("_", "\\");

            cipherText = StringLengthEncode.Decode(cipherText);

            byte[] bytes = Convert.FromBase64String(cipherText);
            using (var memoryStream = new MemoryStream(bytes))
            {
                using (var outputStream = new MemoryStream())
                {
                    using (var decompressStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                        decompressStream.CopyTo(outputStream);

                    return Encoding.ASCII.GetString(outputStream.ToArray());
                }
            }
        }
    }
}
