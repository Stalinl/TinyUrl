namespace TinyUrl.Core
{
    using System;
    using System.Threading.Tasks;

    using EnsureThat;

    public sealed class TinyUrlService : ITinyUrlService
    {
        private readonly IRepository repository;
        private readonly Uri tinyUrlBaseAddress;

        /// <summary>
        /// Initializes a new instance of the <see cref="TinyUrlService"/> class.
        /// </summary>
        /// <param name="repository">The repository service object.</param>
        /// <param name="tinyUrlBaseAddress">The tiny url base address format.</param>
        public TinyUrlService(IRepository repository, Uri tinyUrlBaseAddress)
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
            this.tinyUrlBaseAddress = tinyUrlBaseAddress ?? throw new ArgumentNullException(nameof(tinyUrlBaseAddress));
        }

        /// <inheritdoc/>
        public bool IsTinyUrl(Uri uri)
        {
            EnsureArg.IsNotNull(uri, nameof(uri));

            return string.Equals(uri.Host, this.tinyUrlBaseAddress.Host, StringComparison.OrdinalIgnoreCase);
        }

        /// <inheritdoc/>
        public Task<string> GetOriginalUrlAsync(Uri tinyUri)
        {
            EnsureArg.IsNotNull(tinyUri, nameof(tinyUri));
            return GetOriginalUrlAsync();

            Task<string> GetOriginalUrlAsync()
            {
                if (Helpers.TryDecode(tinyUri.LocalPath?.Substring(1), out int id))
                {
                    return this.repository.GetByIdAsync(id);
                }

                return Task.FromResult(string.Empty);
            }
        }

        /// <inheritdoc/>
        public Task<string> GetTinyUrlAsync(Uri longUri)
        {
            EnsureArg.IsNotNull(longUri, nameof(longUri));
            return GetTinyUrlAsync();

            async Task<string> GetTinyUrlAsync()
            {
                var id = await CreateOrGetUrlId().ConfigureAwait(false);
                var uriBuilder = new UriBuilder(this.tinyUrlBaseAddress);
                uriBuilder.Path = Helpers.Encode(id);

                return uriBuilder.Uri.AbsoluteUri;
            }

            // Check whether the url already exists.
            async Task<int> CreateOrGetUrlId()
            {
                int id = await this.repository.GetByPathAsync(longUri.OriginalString).ConfigureAwait(false);
                return id == 0
                    ? await this.repository.CreateAsync(longUri.OriginalString).ConfigureAwait(false)
                    : id;
            }
        }
    }
}