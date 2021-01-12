using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Taalcafe.Models;
using Taalcafe.Models.DB;
using Taalcafe.Models.ViewModels;

namespace Taalcafe.Controllers
{
    public class CallController : Controller
    {
        private readonly ILogger<CallController> _logger;
        private dbi380705_taalcafeContext context; 

        public CallController(ILogger<CallController> logger)
        {
            _logger = logger; 
        }

        // GET: Call/Nextsession
        public IActionResult NextSession(int? userId)
        {
            Instantiate();

            var Sessies = context.Sessies
                .Where(s => s.Datum >= DateTime.Today)
                .OrderBy(s => s.Datum)
                .ToList();

            /*
            var nextSession = context.Sessies.SingleOrDefault(s => s.Datum >= DateTime.Today);
            ViewBag.session = nextSession;
            */

            ViewData["user"] = userId;
            return View(Sessies.FirstOrDefault());
        }

        // POST: Call/Nextsession
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult NextSession([Bind("Id","Datum","Duur","ThemaId","Aanmeldingen")] Sessie sessie)
        {
            Instantiate();

            context.Entry(sessie).State = EntityState.Modified;
            context.SaveChanges();

            sessie.InitializeAanmeldingIDs();
            ViewData["user"] = sessie.AanmeldingIDs.Last();
            return View(sessie);
        }

        public IActionResult Call(int? id)
        {
            Instantiate();
            SessiePartner sessiePartner = context.SessiePartners
                .Include(c => c.Taalcoach)
                .Include(c => c.Cursist)
                .Include(c => c.Sessie)
                    .ThenInclude(s => s.Thema)
                .SingleOrDefault(c => c.Sessie.Datum == DateTime.Today && (c.TaalcoachId == id || c.CursistId == id));

            if (sessiePartner != null)
            {
                Gebruiker user;
                Gebruiker partner;

                if (sessiePartner.TaalcoachId == id)
                {
                    user = sessiePartner.Taalcoach;
                    partner = sessiePartner.Cursist;
                }
                else {
                    user = sessiePartner.Cursist;
                    partner = sessiePartner.Taalcoach;
                }
            
                CallSessionViewModel viewModel = new CallSessionViewModel(
                    sessiePartner.Sessie.Thema.Naam,
                    sessiePartner.Sessie.Thema.Beschrijving,
                    (int) id,
                    partner.Id,
                    user.Naam,
                    partner.Naam,
                    sessiePartner.Sessie.Thema.Afbeeldingen,
                    sessiePartner.Sessie.Thema.Vragen
                );
                
                return View(viewModel);
            }
            
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Overview()
        {
            Instantiate();
            CallOverviewModel model = new CallOverviewModel() { Gebruikers = new List<Gebruiker>() };
            var gebruikers = context.Gebruikers.Include(g => g.Account);
            foreach (Gebruiker g in gebruikers) {
                model.Gebruikers.Add(new Gebruiker() {
                    Id = g.Id,
                    Naam = g.Naam,
                    Email = g.Email,
                    Telefoon = g.Telefoon,
                    Niveau = g.Niveau,
                    Account = new Account() { Type = g.Account.Type }
                });
            }
            model.Thema = context.Themas.FirstOrDefault(t => t.Sessies.FirstOrDefault(s => s.Thema == t).Datum == DateTime.Today);
            return View(model);
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