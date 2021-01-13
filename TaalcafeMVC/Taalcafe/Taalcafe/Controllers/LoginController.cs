using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Taalcafe.Models;
using Taalcafe.Models.DB;

namespace Taalcafe.Controllers
{
    public class LoginController : Controller
    {
        private dbi380705_taalcafeContext _context;

        public IActionResult SignIn()
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
                var myUser = _context.Accounts
                    .FirstOrDefault(u => u.Gebruikersnaam == model.Gebruikersnaam
                                 && u.Wachtwoord == model.Wachtwoord);

                if (myUser != null)    //User was found
                {
                    //ViewBag.message = "Success";
                    //return View("~/Views/Home/Index.cshtml", model);
                    if(myUser.Type == "Taalcoach" || myUser.Type == "Cursist")
                    {
                        return RedirectToAction("NextSession", "Call", new { userId = myUser.GebruikerId });
                    } else if(myUser.Type == "Coordinator")
                    {
                        return RedirectToAction("Index", "Home");
                    }
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
            _context = new dbi380705_taalcafeContext();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}