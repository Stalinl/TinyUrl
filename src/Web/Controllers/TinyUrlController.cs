namespace Web.Controllers
{
    using System;
    using System.Web.Mvc;

    public class TinyUrlController : Controller
    {
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
        public ActionResult Get(string tinyUrl)
        {
            ViewBag.Result = tinyUrl;
            return View();
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ActionName("Create")]
        public ActionResult Create(string longUrl)
        {
            ViewBag.Result = longUrl;
            return View();
        }
    }
}