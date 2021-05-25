using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taalcafe.DataAccess;
using Taalcafe.Models.DatabaseModels;

namespace Taalcafe.Controllers
{
    [Authorize(Roles = "Coordinator")]
    public class ThemeController : Controller
    {
        private readonly ThemeRepository themeRepository;

        public ThemeController(ThemeRepository themeRepository)
        {
            this.themeRepository = themeRepository;
        }

        public async Task<ActionResult> Index()
        {
            IEnumerable<Theme> model = await themeRepository.GetAllAsync();
            return View(model);
        }

        //public async Task<ActionResult> Details(int id)
        //{
        //    Theme model = await themeRepository.GetByIdAsync(id);
        //    return View(model);
        //}

        public ActionResult Create()
        {
            Theme model = new Theme();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Theme model)
        {
            if (ModelState.IsValid)
            {
                themeRepository.Add(model);
                await themeRepository.SaveAsync();

                TempData["title"] = "Thema toegevoegd!";
                List<string> content = new List<string>();
                content.Add($"Het thema {model.Title} is toegevoegd. ");
                TempData["content"] = content;
                TempData["action"] = "index";
                TempData["controller"] = "theme";

                return RedirectToAction("message", "home");
            }
            else
            {
                return View(model);
            }
        }

        public async Task<ActionResult> Edit(int id)
        {
            Theme model = await themeRepository.GetByIdAsync(id);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, Theme model)
        {
            if (ModelState.IsValid)
            {
                model.Id = id;
                themeRepository.Update(model);
                await themeRepository.SaveAsync();

                TempData["title"] = "Thema bijgewerkt!";
                List<string> content = new List<string>();
                content.Add($"Het thema {model.Title} is bijgewerkt. ");
                TempData["content"] = content;
                TempData["action"] = "index";
                TempData["controller"] = "theme";

                return RedirectToAction("message", "home");
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id)
        {
            Theme theme = await themeRepository.GetByIdAsync(id);

            TempData["title"] = "Thema verwijderd!";
            List<string> content = new List<string>();
            content.Add($"Het thema {theme.Title} is verwijderd. ");
            TempData["content"] = content;
            TempData["action"] = "index";
            TempData["controller"] = "theme";

            themeRepository.Remove(theme);
            await themeRepository.SaveAsync();

            return RedirectToAction("message", "home");
        }
    }
}
