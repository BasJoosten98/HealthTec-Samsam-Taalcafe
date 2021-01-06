using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Taalcafe.Models;
using Taalcafe.Models.DB;

namespace Taalcafe.Controllers
{
    public class SessieController : Controller
    {
        private readonly ILogger<ThemaController> _logger;
        private dbi380705_taalcafeContext context;

        public SessieController(ILogger<ThemaController> logger)
        {
            _logger = logger; 
        }

        public IActionResult Index()
        {
            Instantiate();
            var Sessies = context.Sessies.Include(s => s.Thema).ToList();
            return View(Sessies);
        }

        // GET: Sessie/Create
        public IActionResult Create()
        {
            Instantiate();
            ViewBag.ThemaId = new SelectList(context.Themas, "Id", "Naam");
            return View();
        }

        // POST: Sessie/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Id","Datum","ThemaId")] Sessie sessie) {

            if (ModelState.IsValid)
            {
                Instantiate();
                context.Sessies.Add(sessie);
                context.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ThemaId = new SelectList(context.Themas, "Id", "Naam");
            return View(sessie);
        }

        // GET: Sessie/Edit
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Instantiate();
            Sessie sessie = context.Sessies.Find(id);
            if (sessie == null){
                return NotFound();
            }

            ViewBag.ThemaId = new SelectList(context.Themas, "Id", "Naam", sessie.ThemaId);
            return View(sessie);
        }

        // POST: Thema/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit([Bind("Id","Datum","ThemaId")] Sessie sessie) {

            if (ModelState.IsValid)
            {
                Instantiate();
                context.Entry(sessie).State = EntityState.Modified;
                context.SaveChanges();
                return RedirectToAction("Index");
            }
            
            ViewBag.ThemaId = new SelectList(context.Themas, "Id", "Naam", sessie.ThemaId);
            return View(sessie);
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