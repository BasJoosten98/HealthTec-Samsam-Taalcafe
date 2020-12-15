
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Linq;
using Taalcafe.Models;
using Taalcafe.Models.DB;

namespace Taalcafe.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private dbi380705_taalcafeContext context; 

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(AccountViewModel model)
        {
            Instantiate();
            if (ModelState.IsValid)
            {
                //The ".FirstOrDefault()" method will return either the first matched
                //result or null
                var myUser = context.Accounts
                    .FirstOrDefault(u => u.Gebruikersnaam == model.Gebruikersnaam
                                 && u.Wachtwoord == model.Wachtwoord);

                if (myUser != null)    //User was found
                {
                    //ViewBag.message = "Success";
                    //return View("~/Views/Home/Index.cshtml", model);
                    return RedirectToAction("NextSession", "Call");
                }
                else    //User was not found
                {
                    ModelState.AddModelError("", "Email or password is incorrect");
                    return View("~/Views/Home/Index.cshtml", model);
                }
            }

            return View();
        }

        private void Instantiate()
        {
            context = new dbi380705_taalcafeContext();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
