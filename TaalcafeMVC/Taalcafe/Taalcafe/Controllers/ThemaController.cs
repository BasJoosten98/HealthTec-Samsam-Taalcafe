using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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
            ".jpg", ".jpeg", ".png", ".img" //, ".pdf" (Plans for pdf support)
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
        public async Task<IActionResult> Create([Bind("Id","Naam","Beschrijving","Afbeeldingen","Vragen","Files")] Thema thema) {

            if (ModelState.IsValid)
            {
                Instantiate();

                // check whether the client is trying to upload any unallowed file types.
                if (!ExtensionsAllowed(thema.Files))
                {
                    return View(thema);
                }

                for (int i = 0; i < thema.Files.Count(); i++)
                {
                    var file = thema.Files.ElementAt(i);
                    file.path = GetUniqueFileName(file.file.FileName);

                    await SaveFile(file);
                    
                    if (thema.Afbeeldingen == "" || thema.Afbeeldingen == null)
                    {
                        thema.Afbeeldingen = Path.GetFileName(file.path);
                        
                    }
                    else
                    {
                        thema.Afbeeldingen += ";" + Path.GetFileName(file.path);
                    }
                }

                context.Themas.Add(thema);
                await context.SaveChangesAsync();
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

            if (thema.Afbeeldingen != "" && thema.Afbeeldingen != null)
            {
                var afbeeldinglijst = thema.Afbeeldingen.Split(";");
                foreach (var a in afbeeldinglijst) 
                {
                    thema.Files.Add(new FileModel() {
                        path = a,
                        file = null
                    });
                }
            }

            return View(thema);
        }

        // POST: Thema/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("Id","Naam","Beschrijving","Afbeeldingen","Vragen","Files")] Thema thema) {

            if (ModelState.IsValid)
            {
                Instantiate();

                // check whether the client is trying to upload any unallowed file types.
                if (!ExtensionsAllowed(thema.Files))
                {
                    return View(thema);
                }

                string imageString = null;

                for (int i = 0; i < thema.Files.Count(); i++)
                {
                    var file = thema.Files.ElementAt(i);
                    if (file.path == null) 
                    {
                        file.path = GetUniqueFileName(file.file.FileName);

                        await SaveFile(file);
                    }
                    else if (file.file != null)
                    {
                        file.path = Path.Combine(hostingEnvironment.WebRootPath, "uploads", file.path);
                        System.IO.File.Delete(file.path);
                        file.path = GetUniqueFileName(file.file.FileName);

                        await SaveFile(file);
                    }

                    if (imageString == "" || imageString == null)
                    {
                        imageString = Path.GetFileName(file.path);
                        
                    }
                    else
                    {
                        imageString += ";" + Path.GetFileName(file.path);
                    }
                    
                }

                thema.Afbeeldingen = imageString;
                context.Entry(thema).State = EntityState.Modified;
                await context.SaveChangesAsync();
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
            if (thema.Afbeeldingen != null && thema.Afbeeldingen != "")
            {
                foreach (var image in thema.Afbeeldingen.Split(";"))
                {
                    string path = Path.Combine(hostingEnvironment.WebRootPath, "uploads", image);
                    System.IO.File.Delete(path);
                }
            }
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


        // Checks whether the file types are included in the AllowedExtensions List and thus accepted.
        // Returns true when all file types are accepted and false when one or more isn't.
        private bool ExtensionsAllowed(IEnumerable<FileModel> files)
        {
            foreach (FileModel fm in files) 
            {
                if (!AllowedExtensions.Contains(Path.GetExtension(fm.file.FileName)))
                {
                    return false;
                }
            }
            return true;
        }

        // Creates a new unique filename and path for the file that is to be uploaded.
        private string GetUniqueFileName(string file)
        {
            string extension = Path.GetExtension(file);
            
            if (!AllowedExtensions.Contains(extension)) {
                throw new Exception("Filetype '" + extension + "' is not accepted.");
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

        // Saves the file from the Filemodel to the server.
        private async Task SaveFile(FileModel file)
        {
            using (FileStream fs = new FileStream(file.path, FileMode.Create))
            {
                await file.file.CopyToAsync(fs);
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