using Microsoft.AspNetCore.Mvc;

namespace TaxAppealPlus.Controllers
{
    [Route("/")]
    public class HomeController : Controller
    {
        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("NewYork")]
        public IActionResult NewYork()
        {
            return View();
        }
        [HttpGet("Demo")]
        public IActionResult Demo()
        {
            var referrer = HttpContext.Request.Headers.Referer.ToString();
            ViewData["Referrer"] = string.IsNullOrEmpty(referrer) ? "/" : referrer;
            return View();
        }
        [HttpGet("About")]
        public IActionResult About()
        {
            var referrer = HttpContext.Request.Headers.Referer.ToString();
            ViewData["Referrer"] = string.IsNullOrEmpty(referrer) ? "/" : referrer;
            return View();
        }
    }
}
