using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        public async Task<ActionResult> Index()
        {
            IEnumerable<ApplicationUser> model = await userManager.Users.ToListAsync();
            return View(model);
        }

        public async Task<ActionResult> Details(string id)
        {
            ApplicationUser model = await userManager.FindByIdAsync(id);
            return View(model);
        }

        public ActionResult Create()
        {           
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(UserRegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                //TODO: controleer wie de gebruiker maakt en of de rol van de nieuwe gebruiker is toegestaan

                ApplicationUser user = new ApplicationUser
                {
                    UserName = model.FullName,
                    Email = model.Email,
                    Role = model.Role,
                    PhoneNumber = model.PhoneNumber
                };
                string password = RandomStringGenerator.CreateString(4);

                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    //TODO: laat message zien met wachtwoord
                    return RedirectToAction("index");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }

        public async Task<ActionResult> Edit(string id)
        {
            ApplicationUser user = await userManager.FindByIdAsync(id);
            UserRegisterViewModel model = new UserRegisterViewModel
            {
                FullName = user.UserName,
                PhoneNumber = user.PhoneNumber,
                Role = user.Role,
                Email = user.Email
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(string id, UserRegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = new ApplicationUser
                {
                    UserName = model.FullName,
                    Email = model.Email,
                    Role = model.Role,
                    PhoneNumber = model.PhoneNumber
                };
                await userManager.UpdateAsync(user);

                return RedirectToAction("details", new { id = id});
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(string id)
        {
            ApplicationUser user = await userManager.FindByIdAsync(id);
            await userManager.DeleteAsync(user);
            return RedirectToAction("index");
        }

        [HttpGet]
        public IActionResult Login()
        {
            LoginViewModel model = new LoginViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                //string userName = userManager.Users
                //    .Where(User => User.Email == model.Email)
                //    .FirstOrDefault()?.UserName;

                ApplicationUser user = await userManager.FindByEmailAsync(model.Email);
                string userName = user?.UserName;

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
            return View(model); //Kijk wat verschil is hier!
            //return View();
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("login", "account");
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
        public async Task<IActionResult> Admin(AdminRegisterViewModel model)
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