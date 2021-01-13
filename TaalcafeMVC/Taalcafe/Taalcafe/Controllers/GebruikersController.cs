using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Taalcafe.Models.DB;

namespace Taalcafe.Controllers
{
    public class GebruikersController : Controller
    {
        private readonly ILogger<GebruikersController> _logger;
        private dbi380705_taalcafeContext _context;

        public GebruikersController(ILogger<GebruikersController> logger)
        {
            _logger = logger;
        }

        // GET: Gebruikers
        public async Task<IActionResult> Index()
        {
            Instantiate();
            List<Gebruiker> gebruikers = _context.Gebruikers
                .Include(c => c.Account)
                .ToList();
            return View(gebruikers);
        }

        // GET: Gebruikers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            Instantiate();
            if (id == null)
            {
                return NotFound();
            }

            var gebruiker = await _context.Gebruikers
                .Include(g => g.Account)
                .Include(g => g.SessiePartnerCursists).ThenInclude(sp => sp.Taalcoach)
                .Include(g => g.SessiePartnerCursists).ThenInclude(sp => sp.Sessie)
                .Include(g => g.SessiePartnerTaalcoaches).ThenInclude(sp => sp.Cursist)
                .Include(g => g.SessiePartnerTaalcoaches).ThenInclude(sp => sp.Sessie)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (gebruiker == null)
            {
                return NotFound();
            }
            gebruiker.SessiePartnerCursists.OrderByDescending(sp => sp.Sessie.Datum);
            gebruiker.SessiePartnerTaalcoaches.OrderByDescending(sp => sp.Sessie.Datum);
            
            return View(gebruiker);
        }

        // GET: Gebruikers/Create
        public IActionResult Create()
        {
            ViewBag.roles = new SelectList(new List<string>() {"Coordinator", "Cursist", "Taalcoach"});
            ViewBag.niveaus = new SelectList(new List<string>() {"1", "2", "3"});
            return View();
        }

        // POST: Gebruikers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Naam,Email,Telefoon,Niveau")] Gebruiker gebruiker)
        {
            Instantiate();
            if (ModelState.IsValid)
            {
                _context.Add(gebruiker);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            
            ViewBag.roles = new SelectList(new List<string>() {"Coordinator", "Cursist", "Taalcoach"});
            ViewBag.niveaus = new SelectList(new List<string>() {"1", "2", "3"});
            return View(gebruiker);
        }

        // GET: Gebruikers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            
            Instantiate();
            var gebruiker = await _context.Gebruikers
                .Include(g => g.Account)
                .SingleOrDefaultAsync(m => m.Id == id);
            
            if (gebruiker == null)
            {
                return NotFound();
            }

            ViewBag.roles = new SelectList(new List<string>() {"Coordinator", "Cursist", "Taalcoach"});
            ViewBag.niveaus = new SelectList(new List<string>() {"1", "2", "3"});
            return View(gebruiker);
        }

        // POST: Gebruikers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("Id,Naam,Email,Telefoon,Niveau,Account")] Gebruiker gebruiker)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Instantiate();
                    _context.Update(gebruiker);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GebruikerExists(gebruiker.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", new { id = gebruiker.Id} );
            }

            ViewBag.roles = new SelectList(new List<string>() {"Coordinator", "Cursist", "Taalcoach"});
            ViewBag.niveaus = new SelectList(new List<string>() {"1", "2", "3"});
            return View(gebruiker);
        }

        // GET: Gebruikers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gebruiker = await _context.Gebruikers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (gebruiker == null)
            {
                return NotFound();
            }

            return View(gebruiker);
        }

        // POST: Gebruikers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var gebruiker = await _context.Gebruikers.FindAsync(id);
            _context.Gebruikers.Remove(gebruiker);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GebruikerExists(int id)
        {
            return _context.Gebruikers.Any(e => e.Id == id);
        }

        private void Instantiate()
        {
            _context = new dbi380705_taalcafeContext();
        }
    }
}
