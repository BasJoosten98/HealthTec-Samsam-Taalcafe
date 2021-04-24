using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Taalcafe.Generators;
using Taalcafe.Models.DatabaseModels;
using Taalcafe.Models.Shared;
using Taalcafe.Models.ViewModels;

namespace Taalcafe.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;

        public AccountController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> LoginAsync(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                string userName = userManager.Users
                    .Where(User => User.Email == model.Email)
                    .FirstOrDefault()?.UserName;

                if (string.IsNullOrEmpty(userName))
                {
                    ModelState.AddModelError("", "Er bestaat geen account met dit emailadres");
                }
                else
                {
                    var result = await signInManager.PasswordSignInAsync(userName, model.Password, false, false);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("index", "home");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Verkeerd wachtwoord of emailadres");
                    }
                }

            }
            //return View(model); //Kijk wat verschil is hier!
            return View();
        }

        [HttpGet]
        public IActionResult Admin()
        {
            ApplicationUser adminInDb = userManager.Users
                .Where(user => user.Role == Role.ADMIN)
                .FirstOrDefault();

            if(adminInDb != null)
            {
                return RedirectToAction("index", "home");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AdminAsync(AdminRegisterModelView model)
        {
            ApplicationUser adminInDb = userManager.Users
                .Where(user => user.Role == Role.ADMIN)
                .FirstOrDefault();

            if (adminInDb != null)
            {
                return RedirectToAction("index", "home");
            }

            if (ModelState.IsValid)
            {
                ApplicationUser user = new ApplicationUser
                {
                    UserName = model.FullName,
                    Email = model.Email,
                    Role = Role.ADMIN
                };
                string password = RandomStringGenerator.CreateString(10);

                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("index", "home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }

        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        //public IActionResult Error()
        //{
        //    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        //}
    }
}