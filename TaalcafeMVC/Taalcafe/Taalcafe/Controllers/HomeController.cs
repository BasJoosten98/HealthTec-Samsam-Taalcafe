using Microsoft.AspNetCore.Mvc;

namespace Taalcafe.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Message()
        {
            if(TempData.ContainsKey("title") && TempData.ContainsKey("content"))
            {
                if (!(TempData.ContainsKey("controller") && TempData.ContainsKey("action")))
                {
                    TempData["controller"] = "home";
                    TempData["action"] = "index";
                }
                return View();
            }
            else
            {
                return RedirectToAction("index", "home");
            }
        }

        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        //public IActionResult Error()
        //{
        //    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        //}
    }
}
