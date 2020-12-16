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
                Thema thema = sessiePartner.Sessie.Thema;
                int partnerId;
                string username;
                string partnerName;

                if (sessiePartner.TaalcoachId == id)
                {
                    username = sessiePartner.Taalcoach.Naam;
                    partnerId = sessiePartner.CursistId;
                    partnerName = sessiePartner.Cursist.Naam;
                }
                else {
                    username = sessiePartner.Cursist.Naam;
                    partnerId = sessiePartner.TaalcoachId;
                    partnerName = sessiePartner.Taalcoach.Naam;
                }

                /*
                var couple = new SessiePartner();
                couple.CursistId = sessiePartner.CursistId;
                couple.TaalcoachId = sessiePartner.TaalcoachId;
                ViewBag.user = id;
                ViewBag.couple = couple;
                */
            
                CallSessionViewModel viewModel = new CallSessionViewModel(thema.Naam, thema.Beschrijving, (int) id, partnerId, username, partnerName, thema.Afbeeldingen, thema.Vragen);
                
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