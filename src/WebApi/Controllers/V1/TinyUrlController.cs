namespace TinyUrl.WebApi.Controllers.V1
{
    using System;
    using System.Threading.Tasks;
    using System.Web.Http;

    using EnsureThat;
    using TinyUrl.Core;

    [RoutePrefix("api/v1")]
    public sealed class TinyUrlController : ApiController
    {
        private readonly ITinyUrlService urlService;

        public TinyUrlController(ITinyUrlService urlService)
        {
            this.urlService = EnsureArg.IsNotNull(urlService, nameof(urlService));
        }

        [HttpGet]
        [Route("get")]
        public async Task<IHttpActionResult> GetAsync([FromUri] string url)
        {
            var valid = TryValidateInputUrl(url, out Uri uri);
            if (!valid.Item1)
            {
                return this.BadRequest(valid.Item2);
            }

            if (!this.urlService.IsTinyUrl(uri))
            {
                return this.BadRequest(Resources.InvalidInputTinyUrl);
            }

            var originalUrl = await this.urlService.GetOriginalUrlAsync(uri).ConfigureAwait(false);
            if (string.IsNullOrWhiteSpace(originalUrl))
            {
                return this.NotFound();
            }

            return this.Ok(originalUrl); // this.Redirect(originalUrl)
        }

        [HttpPost]
        [Route("create")]
        public async Task<IHttpActionResult> PostAsync([FromUri] string url)
        {
            var valid = TryValidateInputUrl(url, out Uri uri);
            if (!valid.Item1)
            {
                return this.BadRequest(valid.Item2);
            }

            if (this.urlService.IsTinyUrl(uri))
            {
                return this.BadRequest(Resources.BannedUrlDomain);
            }

            var tinyUrl = await this.urlService.GetTinyUrlAsync(uri).ConfigureAwait(false);
            return this.Ok(tinyUrl);
        }

        private static (bool, string) TryValidateInputUrl(string url, out Uri uri)
        {
            uri = null;
            if (string.IsNullOrWhiteSpace(url))
            {
                return (false, Resources.EmptyUrl);
            }

            if (url.Length > 2048)
            {
                return (false, Resources.TooLongUrl);
            }

            if (!Helpers.TryGetUrl(url, out uri))
            {
                return (false, Resources.InvalidUrl);
            }

            return (true, null);
        }
    }
}