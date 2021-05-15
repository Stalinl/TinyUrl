namespace TinyUrl.Core
{
    using System;
    using System.Threading.Tasks;

    public interface ITinyUrlService
    {
        /// <summary>
        /// Check the given Url is already a tiny url.
        /// </summary>
        /// <param name="uri">The given url</param>
        /// <returns><see langword="true"/> if the given Url is tiny, <see langword="false"/> otherwise.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        bool IsTinyUrl(Uri uri);

        /// <summary>
        /// Get the original Url from tiny Url.
        /// </summary>
        /// <param name="tinyUri">The tiny Url.</param>
        /// <returns>Returns the decoded original Url.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        Task<string> GetOriginalUrlAsync(Uri tinyUri);

        /// <summary>
        /// Get the tiny url for the given long Url.
        /// </summary>
        /// <param name="longUri">The long Url.</param>
        /// <returns>Returns the encoded short url.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        Task<string> GetTinyUrlAsync(Uri longUri);
    }
}