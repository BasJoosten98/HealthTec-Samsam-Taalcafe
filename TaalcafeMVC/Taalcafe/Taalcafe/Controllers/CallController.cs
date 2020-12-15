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
            var couples = context.SessiePartners.Where(c => c.Sessie.Datum == DateTime.Today);

            //ViewData["user"] = context.Gebruikers.SingleOrDefault(g => g.Id == id);
            ViewBag.user = id;
            return View(couples.ToList());
        }

        public IActionResult Overview()
        {
            return View();
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