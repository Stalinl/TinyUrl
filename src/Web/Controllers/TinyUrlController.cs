namespace Web.Controllers
{
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    using EnsureThat;
    using TinyUrl.Core;

    public class TinyUrlController : Controller
    {
        private readonly ITinyUrlService urlService;

        public TinyUrlController(ITinyUrlService urlService)
        {
            this.urlService = EnsureArg.IsNotNull(urlService, nameof(urlService));
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Get()
        {
            return View();
        }

        [HttpPost]
        [ActionName("Get")]
        public async Task<ActionResult> GetAsync(string tinyUrl)
        {
            var valid = TryValidateInputUrl(tinyUrl, out Uri uri);
            if (!valid.Item1)
            {
                ViewBag.Result = valid.Item2;
                ViewBag.AnyFailure = true;
                return View();
            }

            if (!this.urlService.IsTinyUrl(uri))
            {
                ViewBag.Result = Resources.InvalidInputTinyUrl;
                ViewBag.AnyFailure = true;
                return View();
            }

            var originalUrl = await this.urlService.GetOriginalUrlAsync(uri).ConfigureAwait(false);
            if (string.IsNullOrWhiteSpace(originalUrl))
            {
                ViewBag.Result = "NotFound";
                ViewBag.AnyFailure = true;
                return View();
            }

            ViewBag.Result = originalUrl;
            return View();
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ActionName("Create")]
        public async Task<ActionResult> CreateAsync(string longUrl)
        {
            var valid = TryValidateInputUrl(longUrl, out Uri uri);
            if (!valid.Item1)
            {
                ViewBag.Result = valid.Item2;
                ViewBag.AnyFailure = true;
                return View();
            }

            if (this.urlService.IsTinyUrl(uri))
            {
                ViewBag.Result = Resources.BannedUrlDomain;
                ViewBag.AnyFailure = true;
                return View();
            }

            var tinyUrl = await this.urlService.GetTinyUrlAsync(uri).ConfigureAwait(false);

            ViewBag.Result = tinyUrl;
            return View();
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