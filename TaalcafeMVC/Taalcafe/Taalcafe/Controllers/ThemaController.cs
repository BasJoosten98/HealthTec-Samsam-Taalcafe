using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Taalcafe.Models;
using Taalcafe.Models.DB;
using Taalcafe.Models.ViewModels;

namespace Taalcafe.Controllers
{
    public class ThemaController : Controller
    {
        private readonly ILogger<ThemaController> _logger;
        private dbi380705_taalcafeContext context;
        private readonly IWebHostEnvironment hostingEnvironment;
        private static readonly List<string> AllowedExtensions = new List<string>() {
            ".jpg", ".jpeg", ".png", ".img", ".pdf"
        };

        public ThemaController(IWebHostEnvironment environment, ILogger<ThemaController> logger)
        {
            _logger = logger;
            hostingEnvironment = environment; 
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
        public IActionResult Create([Bind("Id","Naam","Beschrijving","Afbeeldingen","Vragen","Files")] Thema thema) {

            if (ModelState.IsValid)
            {
                Instantiate();

                for (int i = 0; i < thema.Files.Count(); i++)
                {
                    var file = thema.Files.ElementAt(i);
                    file.path = GetUniqueFileName(file.file.FileName);
                    if (file.path == null) 
                    {
                        foreach (var f in thema.Files) 
                        {
                            if (f.path != null) 
                            {
                                System.IO.File.Delete(f.path);
                                thema.Files.FirstOrDefault(tf => tf.path == f.path).path = null;
                            }
                        }
                        return View(thema);
                    }
                    else 
                    {
                        using (FileStream fs = new FileStream(file.path, FileMode.Create))
                        {
                            file.file.CopyTo(fs);
                        }
                        
                        if (thema.Afbeeldingen == "" || thema.Afbeeldingen == null)
                        {
                            thema.Afbeeldingen = Path.GetFileName(file.path);
                            
                        }
                        else
                        {
                            thema.Afbeeldingen += ";" + Path.GetFileName(file.path);
                        }
                    }
                }


                context.Themas.Add(thema);
                context.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(thema);
        }

        // GET: Thema/Edit
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Instantiate();
            Thema thema = context.Themas.Find(id);
            if (thema == null){
                return NotFound();
            }

            return View(thema);
        }

        // POST: Thema/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit([Bind("Id","Naam","Beschrijving","Afbeeldingen","Vragen","Files")] Thema thema) {

            if (ModelState.IsValid)
            {
                Instantiate();

                context.Entry(thema).State = EntityState.Modified;
                context.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(thema);
        }

        // GET: Thema/Delete
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Instantiate();
            Thema thema = context.Themas.Include(t => t.Sessies).SingleOrDefault(t => t.Id == id);
            if (thema == null){
                return NotFound();
            }

            return View(thema);
        }

        // POST: Thema/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id) {
            Instantiate();
            Thema thema = context.Themas.Find(id);
            context.Themas.Remove(thema);
            context.SaveChanges();
            return RedirectToAction("Index");
        }

        // POST: Thema/AddFile
        // For rendering partial view into Thema/Create and Thema/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddFile([Bind("Files")] Thema thema)
        {
            thema.Files.Add(new FileModel());
            return PartialView("Files", thema);
        }

        private string GetUniqueFileName(string file)
        {
            string extension = Path.GetExtension(file);
            
            if (!AllowedExtensions.Contains(extension)) {
                return null;
            }

            string uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "uploads");
            string fileName = "file_1" + extension;

            for (int num = 2; true ; num++ )
            {
                string filePath = Path.Combine(uploadsFolder, fileName);
                if (!System.IO.File.Exists(filePath))
                {
                    return filePath;
                }
                fileName = "file_" + num.ToString() + extension;
            }
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