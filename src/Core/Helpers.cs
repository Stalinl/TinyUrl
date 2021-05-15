namespace TinyUrl.Core
{
    using System;
    using System.Text;

    using EnsureThat;

    public static class Helpers
    {
        /// <summary>
        /// Get encoded string.
        /// </summary>
        /// <param name="id">Value to encode.</param>
        /// <returns>Base64 encoded string.</returns>
        public static string Encode(int id)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(id.ToString());
            return Convert.ToBase64String(plainTextBytes);
        }

        /// <summary>
        /// Converts the specified base64 encoded string to int.
        /// </summary>
        /// <param name="encodedData">Encoded value.</param>
        /// <param name="id">Decoded int value.</param>
        /// <returns><see langword="true"/> if successfully decoded, <see langword="false"/> otherwise.</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool TryDecode(string encodedData, out int id)
        {
            EnsureArg.IsNotNullOrWhiteSpace(encodedData, nameof(encodedData));

            try
            {
                var base64EncodedBytes = Convert.FromBase64String(encodedData);
                var decoded = Encoding.UTF8.GetString(base64EncodedBytes);
                return int.TryParse(decoded, out id);
            }
            catch
            {
                id = 0;
                return false;
            }
        }

        /// <summary>
        /// Converts the string representation to Url.
        /// </summary>
        /// <param name="urlString">A string containing url.</param>
        /// <param name="uri">When this method returns, contains the constructed Url.</param>
        /// <returns><see langword="true"/> if urlString was converted successfully; <see langword="false"/> otherwise.</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool TryGetUrl(string urlString, out Uri uri)
        {
            EnsureArg.IsNotNullOrWhiteSpace(urlString, nameof(urlString));

            if (string.IsNullOrWhiteSpace(urlString) ||
                !Uri.IsWellFormedUriString(urlString, UriKind.Absolute) ||
                !Uri.TryCreate(urlString, UriKind.Absolute, out uri))
            {
                uri = null;
                return false;
            }

            return true;
        }
    }
}