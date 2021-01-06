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

        // GET: Sessie/Delete
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Instantiate();
            Sessie sessie = context.Sessies
                .Include(s => s.Thema)
                .Include(s => s.SessiePartners).ThenInclude(p => p.Taalcoach)
                .Include(s => s.SessiePartners).ThenInclude(p => p.Cursist)
                .SingleOrDefault(s => s.Id == id);
            if (sessie == null){
                return NotFound();
            }

            return View(sessie);
        }

        // POST: Sessie/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id) {
            Instantiate();
            Sessie sessie = context.Sessies
                .Include(s => s.SessiePartners)
                .SingleOrDefault(s => s.Id == id);
            
            context.SessiePartners.RemoveRange(sessie.SessiePartners);
            context.Sessies.Remove(sessie);
            context.SaveChanges();
            
            return RedirectToAction("Index");
        }

        // GET: Sessie/Couples
        public IActionResult Couples(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Instantiate();
            Sessie sessie = context.Sessies
                .Include(s => s.SessiePartners)
                .SingleOrDefault(s => s.Id == id);

            if (sessie == null){
                return NotFound();
            }

            /*
            ViewBag.sessie = sessie;
            
            List<SessiePartner> duos = context.SessiePartners
                .Include(d => d.Sessie)
                .Include(d => d.Taalcoach)
                .Include(d => d.Cursist)
                .Where(d => d.SessieId == sessie.Id)
                .ToList();
            */
            
            List<Gebruiker> taalcoaches = context.Gebruikers
                .Include(g => g.Account)
                .Where(g => g.Account.Type.ToLower() == "taalcoach" || g.Account.Type == "coordinator")
                .ToList();
            
            List<Gebruiker> cursisten = context.Gebruikers
                .Include(g => g.Account)
                .Where(g => g.Account.Type.ToLower() == "cursist")
                .ToList();

            ViewBag.Taalcoaches = new SelectList(taalcoaches, "Id", "Naam");
            ViewBag.Cursisten = new SelectList(cursisten, "Id", "Naam");

            return View(sessie);
        }

        // POST: Sessie/Couples
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Couples([Bind("Id,Items")] Sessie sessie)
        {
            if (ModelState.IsValid)
            {
                Instantiate();
                context.Entry(sessie).State = EntityState.Modified;
                context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(sessie);
        }

        // POST: Sessie/AddDuo
        // For rendering partial view into Sessie/Couples
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddDuo([Bind("SessiePartners")] Sessie sessie)
        {
            sessie.SessiePartners.Add(new SessiePartner());
            return PartialView("SessiePartner", sessie);
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