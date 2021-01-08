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

            var Sessies = context.Sessies
                .Include(s => s.Thema)
                .Where(s => s.Datum >= DateTime.Now.Date)
                .OrderBy(s => s.Datum)
                .ToList();

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
                .Include(s => s.SessiePartners).ThenInclude(p => p.Taalcoach)
                .Include(s => s.SessiePartners).ThenInclude(p => p.Cursist)
                .SingleOrDefault(s => s.Id == id);

            if (sessie == null){
                return NotFound();
            }

            ViewBag.Taalcoaches = GetTaalcoachSelectList();
            ViewBag.Cursisten = GetCursistSelectList();

            return View(sessie);
        }

        // POST: Sessie/Couples
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Couples([Bind("Id","Datum","ThemaId","SessiePartners")] Sessie sessie)
        {
            //TODO: proper model validation for both Sessie and SessiePartner
            if (ModelState.IsValid)
            {
                Instantiate();

                foreach (SessiePartner p in context.SessiePartners) {
                    if (p.SessieId == sessie.Id) {
                        context.SessiePartners.Remove(p);
                    }
                }

                for (int i = 0; i < sessie.SessiePartners.Count(); i++)
                {
                    SessiePartner p = sessie.SessiePartners.ElementAt(i);

                    if (sessie.SessiePartners.Where(s => s.TaalcoachId == p.TaalcoachId).Count() > 1
                        || sessie.SessiePartners.Where(s => s.CursistId == p.CursistId).Count() > 1)
                    {
                        // invalid input, a user was used in multiple inputs
                        ViewBag.Taalcoaches = GetTaalcoachSelectList();
                        ViewBag.Cursisten = GetCursistSelectList();
                        ViewBag.invalid = true;

                        return View(sessie);
                    }
                }

                for (int i = 0; i < sessie.SessiePartners.Count(); i++)
                {
                    SessiePartner p = sessie.SessiePartners.ElementAt(i);

                    if (p.CursistId == 0 || p.TaalcoachId == 0) {
                        sessie.SessiePartners.Remove(p);
                        i -= 1;
                        continue;
                    }

                    context.SessiePartners.Add(p);
                }

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
        public ActionResult AddDuo([Bind("Id,SessiePartners")] Sessie sessie)
        {            
            ViewBag.Taalcoaches = GetTaalcoachSelectList();
            ViewBag.Cursisten = GetCursistSelectList();

            sessie.SessiePartners.Add(new SessiePartner());
            return PartialView("SessiePartners", sessie);
        }
        

        private void Instantiate()
        {
            context = new dbi380705_taalcafeContext();
        }

        private SelectList GetCursistSelectList() 
        {
            Instantiate();

            List<Gebruiker> cursisten = context.Gebruikers
                .Include(g => g.Account)
                .Where(g => g.Account.Type.ToLower() == "cursist")
                .ToList();

            cursisten.Add(new Gebruiker() {
                Id = 0,
                Naam = "Selecteer een Cursist"
            });

            return new SelectList(cursisten, "Id", "Naam");
        }

        private SelectList GetTaalcoachSelectList() 
        {
            Instantiate();

            List<Gebruiker> taalcoaches = context.Gebruikers
                .Include(g => g.Account)
                .Where(g => g.Account.Type.ToLower() == "taalcoach" || g.Account.Type == "coordinator")
                .ToList();

            taalcoaches.Add(new Gebruiker() {
                Id = 0,
                Naam = "Selecteer een Taalcoach"
            });

            return new SelectList(taalcoaches, "Id", "Naam");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}