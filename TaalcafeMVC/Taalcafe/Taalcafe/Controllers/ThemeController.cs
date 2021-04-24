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

        public async Task<ActionResult> Details(int id)
        {
            Theme model = await themeRepository.GetByIdAsync(id);
            return View(model);
        }

        public ActionResult Create()
        {
            Theme model = new Theme();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Theme createdTheme)
        {
            if (ModelState.IsValid)
            {
                themeRepository.Add(createdTheme);
                await themeRepository.SaveAsync();

                return RedirectToAction("details", new { id = createdTheme.Id });
            }
            else
            {
                return View();
            }
        }

        public async Task<ActionResult> Edit(int id)
        {
            Theme model = await themeRepository.GetByIdAsync(id);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, Theme updatedTheme)
        {
            if (ModelState.IsValid)
            {
                await themeRepository.SaveAsync();

                return RedirectToAction("details", new { id = updatedTheme.Id });
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
            themeRepository.Remove(theme);
            await themeRepository.SaveAsync();
            return RedirectToAction("index");
        }
    }
}
