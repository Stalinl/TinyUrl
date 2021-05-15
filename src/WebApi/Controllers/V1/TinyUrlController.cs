namespace TinyUrl.WebApi.Controllers.V1
{
    using System.Web.Http;

    using EnsureThat;
    using TinyUrl.Core;

    [RoutePrefix("api/v1")]
    public class TinyUrlController : ApiController
    {
        private readonly ITinyUrlService urlService;

        public TinyUrlController(ITinyUrlService urlService)
        {
            this.urlService = EnsureArg.IsNotNull(urlService, nameof(urlService));
        }

        // GET
        public IHttpActionResult Index()
        {
            return this.Ok();
        }
    }
}