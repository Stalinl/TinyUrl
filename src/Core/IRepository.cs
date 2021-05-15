namespace TinyUrl.Core
{
    using System.Threading.Tasks;

    public interface IRepository
    {
        /// <summary>
        /// Serch by id and get original url string.
        /// </summary>
        /// <param name="id">The id to search for.</param>
        /// <returns>A Url string.</returns>
        Task<string> GetByIdAsync(int id);

        /// <summary>
        /// Serch by original url string and get id.
        /// </summary>
        /// <param name="originalUrl">The original url string to search.</param>
        /// <returns>The Id.</returns>
        Task<int> GetByPathAsync(string originalUrl);

        /// <summary>
        /// Create and return the id.
        /// </summary>
        /// <param name="urlString">The long Url.</param>
        /// <returns>The created id.</returns>
        Task<int> CreateAsync(string shortUrl);
    }
}