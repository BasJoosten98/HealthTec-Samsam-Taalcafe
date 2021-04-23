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

            ViewData["user"] = userId;
            return View(Sessies.FirstOrDefault());
        }

        // POST: Call/Nextsession
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NextSession([Bind("Id","Datum","Duur","ThemaId","Aanmeldingen")] Sessie sessie)
        {
            Instantiate();
            
            var dbvalues = await context.Entry(sessie).GetDatabaseValuesAsync();
            var aanmeldingprop = dbvalues.Properties.FirstOrDefault(p => p.Name == "Aanmeldingen");
            string dbAanmeldingen = dbvalues.GetValue<string>(aanmeldingprop);
            List<int> dbAanmeldingList = new List<int>();
            if (dbAanmeldingen != null && dbAanmeldingen != "")
            {
                foreach (var id in dbAanmeldingen.Split(","))
                {
                    dbAanmeldingList.Add(Int32.Parse(id));
                }
            }
            
            sessie.InitializeAanmeldingIDs();
            if (dbAanmeldingList.Count() >= sessie.AanmeldingIDs.Count())
            {
                foreach (int id in dbAanmeldingList)
                {
                    if (!sessie.AanmeldingIDs.Contains(id))
                    {
                        sessie.Aanmeldingen += "," + id.ToString();
                    }
                }
            }            

            context.Entry(sessie).State = EntityState.Modified;
            await context.SaveChangesAsync();

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
                .SingleOrDefault(c => c.Sessie.Datum.Value.Date == DateTime.Today && (c.TaalcoachId == id || c.CursistId == id));

            CallSessionViewModel viewModel;
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
            
                 viewModel = new CallSessionViewModel(
                    sessiePartner.SessieId,
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
             viewModel = new CallSessionViewModel(
                    2,
                    "Koken",
                    "Koken is het bereiden van voedsel door middel van verhitting! Door deze voedselverwerkingsmethode verandert de structuur van het rauwe voedsel en worden bacteriën gedood",
                    8,
                    7,
                    "Bas",
                    "Ralf",
                    null,
                    "Wat is je favoriete gerecht om klaar te maken?~Draag je een koksmuts tijdens het koken?~Doe je het samen?"
                );
            return View(viewModel);

            return RedirectToAction("SignIn", "Login");
        }

        public IActionResult NewCall()
        {
            CallSessionViewModel viewModel = new CallSessionViewModel(
                    2,
                    "Koken",
                    "Koken is het bereiden van voedsel door middel van verhitting! Door deze voedselverwerkingsmethode verandert de structuur van het rauwe voedsel en worden bacteriën gedood",
                    8,
                    7,
                    "Bas",
                    "Ralf",
                    null,
                    "Wat is je favoriete gerecht om klaar te maken?~Draag je een koksmuts tijdens het koken?~Doe je het samen?"
                );
            return View(viewModel);
        }

        public IActionResult Overview()
        {
            Instantiate();
            CallOverviewModel model = new CallOverviewModel() { Gebruikers = new List<Gebruiker>() };
            var gebruikers = context.Gebruikers.Include(g => g.Account).Where(g => g.Account != null);
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

        // GET: Call/SessionEvaluation
        public async Task<IActionResult> SessionEvaluation(int? userId, int? sessionId)
        {
            if (userId != null && sessionId != null)
            {
                Instantiate();

                SessiePartner sp = await context.SessiePartners.SingleOrDefaultAsync(sp => 
                    sp.SessieId == sessionId && (sp.TaalcoachId == userId || sp.CursistId == userId)
                );
                
                if (sp != null)
                {
                    SessionEvaluationViewModel sevm = new SessionEvaluationViewModel() {
                        SessieId = sp.SessieId,
                        GebruikersId = (int) userId,
                    };
                    
                    return View(sevm);
                }
            }

            return RedirectToAction("SignIn", "Login");
        }

        // POST: Call/SessionEvaluation
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SessionEvaluation([Bind("SessieId","GebruikersId","Cijfer","Feedback")] SessionEvaluationViewModel sevm)
        {
            if (ModelState.IsValid)
            {
                Instantiate();

                SessiePartner sp = await context.SessiePartners.SingleOrDefaultAsync(sp => 
                    sp.SessieId == sevm.SessieId && (sp.TaalcoachId == sevm.GebruikersId || sp.CursistId == sevm.GebruikersId)
                );

                if (sp.TaalcoachId == sevm.GebruikersId)
                {
                    sp.CijferTaalcoach = sevm.Cijfer;
                    sp.FeedbackTaalcoach = sevm.Feedback;
                }
                else
                {
                    sp.CijferCursist = sevm.Cijfer;
                    sp.FeedbackCursist = sevm.Feedback;
                }

                context.Entry(sp).State = EntityState.Modified;
                context.SaveChanges();

                return RedirectToAction("NextSession", new { userId=sevm.GebruikersId } );
            }
            return View(sevm);
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