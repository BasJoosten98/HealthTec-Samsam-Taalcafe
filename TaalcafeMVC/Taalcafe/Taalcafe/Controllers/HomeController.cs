using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Taalcafe.DataAccess;
using Taalcafe.Models.DatabaseModels;

namespace Taalcafe.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly UserEntryRepository userEntryRepository;

        public HomeController(UserManager<ApplicationUser> userManager, UserEntryRepository userEntryRepository)
        {
            this.userManager = userManager;
            this.userEntryRepository = userEntryRepository;
        }

        public async Task<IActionResult> Index()
        {
            TempData["message"] = " ";

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(userId))
            {
                var user = await userManager.FindByIdAsync(userId);
                var roles = await userManager.GetRolesAsync(user);
                if (roles.Count > 0)
                {
                    if (roles[0].ToLower() == "cursist" || roles[0].ToLower() == "taalcoach")
                    {
                        IEnumerable<UserEntry> entries = await userEntryRepository.GetByUserIdIncludingMeetingAsync(userId);
                        TempData["message"] = "U heeft zich nog iet aangemeld voor een komende meeting. Ga naar meetings beheren om je aan te melden!";
                        UserEntry closest = null;
                        foreach (UserEntry e in entries)
                        {
                            if (e.Meeting.EndDate < DateTime.Now) { continue; }
                            else if (e.Meeting.StartDate <= DateTime.Now && e.Meeting.EndDate > DateTime.Now)
                            {
                                TempData["message"] = "Een taalcafe meeting is begonnen! Klik op videobellen om deel te nemen!";
                                closest = null;
                                break;
                            }
                            else
                            {
                                if (closest == null) { closest = e; }
                                else if (closest.Meeting.StartDate > e.Meeting.StartDate)
                                {
                                    closest = e;
                                }
                            }
                        }
                        if (closest != null)
                        {
                            TempData["message"] = "Uw eerst volgende taalcafe meeting vind plaats op " + closest.Meeting.StartDate.ToString("dd-MM-yyyy HH:mm") + ". Tot dan!";
                        }
                    }
                }
            }
            
            return View();
        }

        [AllowAnonymous]
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
