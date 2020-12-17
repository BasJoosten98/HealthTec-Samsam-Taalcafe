using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Taalcafe.Models.DB;
using Taalcafe.Models;

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

        public IActionResult NextSession(int? id)
        {
            Instantiate();
            var nextSession = context.Sessies.SingleOrDefault(s => s.Datum >= DateTime.Today);
            ViewBag.session = nextSession;
            
            ViewData["user"] = id;
            return View();
        }

        public IActionResult Call(int? id)
        {
            Instantiate();
            SessiePartner sessiePartner = context.SessiePartners.SingleOrDefault(c => c.Sessie.Datum == DateTime.Today && (c.TaalcoachId == id || c.CursistId == id));
            if (sessiePartner != null)
            {
                Sessie sessie = context.Sessies.SingleOrDefault(s => s.Id == sessiePartner.SessieId);
                Thema thema = context.Themas.SingleOrDefault(t => t.Id == sessie.ThemaId);
                Gebruiker user = context.Gebruikers.SingleOrDefault(g => g.Id == id);
                Gebruiker partner;

                if (sessiePartner.TaalcoachId == id)
                {
                    partner = context.Gebruikers.SingleOrDefault(g => g.Id == sessiePartner.CursistId);
                }
                else {
                    partner = context.Gebruikers.SingleOrDefault(g => g.Id == sessiePartner.TaalcoachId);
                }
            
                CallSessionViewModel viewModel = new CallSessionViewModel(thema.Naam, thema.Beschrijving, (int) id, partner.Id, user.Naam, partner.Naam, thema.Afbeeldingen, thema.Vragen);
                
                return View(viewModel);
            }
            
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Overview()
        {
            Instantiate();
            return View(context.Gebruikers.ToList());
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