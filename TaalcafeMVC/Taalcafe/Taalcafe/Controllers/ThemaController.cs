using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Taalcafe.Models;
using Taalcafe.Models.DB;

namespace Taalcafe.Controllers
{
    public class ThemaController : Controller
    {
        private readonly ILogger<ThemaController> _logger;
        private dbi380705_taalcafeContext context;

        public ThemaController(ILogger<ThemaController> logger)
        {
            _logger = logger; 
        }

        public IActionResult Index()
        {
            Instantiate();
            var themas = context.Themas.ToList();
            return View(themas);
        }

        // GET: Thema/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Thema/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Id","Naam","Beschrijving","Afbeeldingen","Vragen")] Thema thema) {

            if (ModelState.IsValid)
            {
                Instantiate();
                context.Themas.Add(thema);
                context.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(thema);
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