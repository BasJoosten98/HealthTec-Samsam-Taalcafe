using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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

        [Authorize(Roles = "Admin, Coordinator")]
        public async Task<ActionResult> Index()
        {
            IEnumerable<ApplicationUser> model = await userManager.Users.ToListAsync();
            model = model.OrderBy(user => user.UserName.ToLower());
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            UserRegisterViewModel model = new UserRegisterViewModel();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Create(UserRegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                //TODO: controleer wie de gebruiker maakt en of de rol van de nieuwe gebruiker is toegestaan

                ApplicationUser user = new ApplicationUser
                {
                    UserName = model.FullName,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber
                };
                string password = RandomStringGenerator.CreateString(4);

                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, model.Role.ToString());
                    
                    TempData["title"] = "Gebruiker toegevoegd!";
                    List<string> content = new List<string>();
                    content.Add("Uw gebruikers account is gemaakt met de gegeven gegevens.");
                    content.Add("");
                    content.Add("Email: " + model.Email);
                    content.Add("Wachtwoord: " + password);
                    content.Add("");
                    content.Add("U kunt nu gebruik maken van het account.");
                    TempData["content"] = content;
                    TempData["action"] = "index";
                    TempData["controller"] = "account";
                    return RedirectToAction("message", "home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Edit(string id)
        {
            ApplicationUser user = await userManager.FindByIdAsync(id);
            var roles = await userManager.GetRolesAsync(user);
            UserRegisterViewModel model = new UserRegisterViewModel
            {
                FullName = user.UserName,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                Role = Enum.Parse<Role>(roles[0])
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Edit(string id, UserRegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByIdAsync(id);
                var roles = await userManager.GetRolesAsync(user);
                if(roles[0] != model.Role.ToString())
                {
                    await userManager.RemoveFromRoleAsync(user, roles[0]);
                    await userManager.AddToRoleAsync(user, model.Role.ToString());
                }

                user.UserName = model.FullName;
                user.Email = model.Email;
                user.PhoneNumber = model.PhoneNumber;

                await userManager.UpdateAsync(user);

                TempData["title"] = "Gebruiker bijgewerkt!";
                List<string> content = new List<string>();
                content.Add($"De gebruiker {user.UserEntries} is bijgewerkt.");
                TempData["content"] = content;
                TempData["action"] = "index";
                TempData["controller"] = "account";

                return RedirectToAction("message", "home");
            }
            else
            {
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(string id)
        {
            ApplicationUser user = await userManager.FindByIdAsync(id);

            TempData["title"] = "Gebruiker verwijderd!";
            List<string> content = new List<string>();
            content.Add($"De gebruiker {user.UserName} is verwijderd. ");
            TempData["content"] = content;
            TempData["action"] = "index";
            TempData["controller"] = "account";

            await userManager.DeleteAsync(user);

            return RedirectToAction("message", "home");
        }

        [HttpGet]
        public IActionResult Login()
        {
            LoginViewModel model = new LoginViewModel();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = await userManager.FindByEmailAsync(model.Email);
                string userName = user?.UserName;

                if (string.IsNullOrEmpty(userName))
                {
                    ModelState.AddModelError("", "Verkeerd wachtwoord of emailadres"); //geen account met deze email
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
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("login");
        }

        [HttpGet]
        public async Task<IActionResult> Admin()
        {
            var adminUsers = await userManager.GetUsersInRoleAsync("Admin");
            if(adminUsers != null && adminUsers.Count > 0)
            {
                return RedirectToAction("index", "home");
            }
            AdminRegisterViewModel model = new AdminRegisterViewModel();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Admin(AdminRegisterViewModel model)
        {
            var adminUsers = await userManager.GetUsersInRoleAsync("Admin");
            if (adminUsers != null && adminUsers.Count > 0)
            {
                return RedirectToAction("index", "home");
            }

            if (ModelState.IsValid)
            {
                ApplicationUser user = new ApplicationUser
                {
                    UserName = model.FullName,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber
                };
                string password = RandomStringGenerator.CreateString(10);

                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Admin");
                    await signInManager.SignInAsync(user, isPersistent: false);
                    TempData["title"] = "Admin account gemaakt!";
                    List<string> content = new List<string>();
                    content.Add("Uw admin account is gemaakt met de gegeven gegevens.");
                    content.Add("");
                    content.Add("Email: " + model.Email);
                    content.Add("Wachtwoord: " + password);
                    content.Add("");
                    content.Add("U kunt nu gebruik maken van uw admin account.");
                    TempData["content"] = content;
                    return RedirectToAction("message", "home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ResetPassword(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            var token = await userManager.GeneratePasswordResetTokenAsync(user);

            string newPassword = null;
            var roles = await userManager.GetRolesAsync(user);
            if(roles[0] == "Admin") { newPassword = RandomStringGenerator.CreateString(10); }
            else if (roles[0] == "Coordinator") { newPassword = RandomStringGenerator.CreateString(6); }
            else { newPassword = RandomStringGenerator.CreateString(4); }

            var result = await userManager.ResetPasswordAsync(user, token, newPassword);
            if (result.Succeeded)
            {
                TempData["title"] = "Nieuw wachtwoord!";
                List<string> content = new List<string>();
                content.Add("Het account is bijgewerkt met een nieuw wachtwoord.");
                content.Add("");
                content.Add("Email: " + user.Email);
                content.Add("Wachtwoord: " + newPassword);
                content.Add("");
                content.Add("U kunt nu gebruik maken van het account met het nieuwe wachtwoord.");
                TempData["content"] = content;
                TempData["action"] = "index";
                TempData["controller"] = "account";
                return RedirectToAction("message", "home");
            }
            TempData["title"] = "Mislukt!";
            List<string> content2 = new List<string>();
            content2.Add("Het account is NIET bijgewerkt met een nieuw wachtwoord.");
            content2.Add("Probeer het later nog eens.");
            TempData["content"] = content2;
            TempData["action"] = "index";
            TempData["controller"] = "account";
            return RedirectToAction("message", "home");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        //public IActionResult Error()
        //{
        //    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        //}
    }
}