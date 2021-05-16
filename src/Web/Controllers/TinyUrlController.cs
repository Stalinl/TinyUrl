namespace TinyUrl.Web.Controllers
{
    using System;
    using System.Web.Mvc;

    public class TinyUrlController : Controller
    {
        // GET: TinyUrl
        public ActionResult Index()
        {
            return View();
        }
    }
}