namespace TinyUrl.WebApi.Controllers.V1
{
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
            return this.Ok(url);
        }

        [HttpPost]
        [Route("create")]
        public async Task<IHttpActionResult> PostAsync([FromUri] string url)
        {
            return this.Ok(url);
        }
    }
}