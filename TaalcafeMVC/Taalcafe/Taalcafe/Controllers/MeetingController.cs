using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Taalcafe.DataAccess;
using Taalcafe.Models.DatabaseModels;
using Taalcafe.Models.ViewModels;

namespace Taalcafe.Controllers
{
    public class MeetingController : Controller
    {
        private readonly MeetingRepository meetingRepository;
        private readonly UserEntryRepository userEntryRepository;
        private readonly ThemeRepository themeRepository;

        public MeetingController(MeetingRepository meetingRepository,
            UserEntryRepository userEntryRepository,
            ThemeRepository themeRepository)
        {
            this.meetingRepository = meetingRepository;
            this.userEntryRepository = userEntryRepository;
            this.themeRepository = themeRepository;
        }

        public async Task<ActionResult> Index()
        {
            IEnumerable<Meeting> model = await meetingRepository.GetAllMeetingsIncludingThemes();
            return View(model);
        }

        public async Task<ActionResult> Details(int id)
        {
            Meeting model = await meetingRepository.GetByIdAsync(id);
            return View(model);
        }

        public async Task<ActionResult> Create()
        {
            IEnumerable<Theme> themes = await themeRepository.GetAllAsync();
            CreateMeetingViewModel model = new CreateMeetingViewModel
            {
                StartDate = DateTime.Now + TimeSpan.FromMinutes(5),
                EndDate = DateTime.Now + TimeSpan.FromHours(1) + TimeSpan.FromMinutes(5),
                ThemeSelectList = themes.Select(theme => new SelectListItem(theme.Title, theme.Id.ToString()))
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Meeting model)
        {
            if (ModelState.IsValid)
            {
                meetingRepository.Add(model);
                await meetingRepository.SaveAsync();

                TempData["title"] = "Meeting toegevoegd!";
                List<string> content = new List<string>();
                content.Add("De meeting is toegevoegd. ");
                TempData["content"] = content;
                TempData["action"] = "index";
                TempData["controller"] = "meeting";

                return RedirectToAction("message", "home");
            }
            else
            {
                return View();
            }
        }

        public async Task<ActionResult> Edit(int id)
        {
            IEnumerable<Theme> themes = await themeRepository.GetAllAsync();
            Meeting meeting = await meetingRepository.GetByIdAsync(id);
            CreateMeetingViewModel model = new CreateMeetingViewModel
            {
                ThemeId = meeting.ThemeId,
                StartDate = meeting.StartDate,
                EndDate = meeting.EndDate,
                ThemeSelectList = themes.Select(theme => new SelectListItem(theme.Title, theme.Id.ToString()))
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, Meeting model)
        {
            if (ModelState.IsValid)
            {
                model.Id = id;
                meetingRepository.Update(model);
                await meetingRepository.SaveAsync();

                TempData["title"] = "Meeting bijgewerkt!";
                List<string> content = new List<string>();
                content.Add("De meeting is bijgewerkt. ");
                TempData["content"] = content;
                TempData["action"] = "index";
                TempData["controller"] = "meeting";

                return RedirectToAction("message", "home");
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id)
        {
            Meeting meeting = await meetingRepository.GetByIdAsync(id);

            TempData["title"] = "Meeting verwijderd!";
            List<string> content = new List<string>();
            content.Add("De meeting is verwijderd. ");
            TempData["content"] = content;
            TempData["action"] = "index";
            TempData["controller"] = "meeting";

            meetingRepository.Remove(meeting);
            await meetingRepository.SaveAsync();

            return RedirectToAction("message", "home");
        }

        [HttpGet]
        public async Task<IActionResult> Feedback(int id)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            UserEntry entry = await userEntryRepository.GetByUserIdAndMeetingIdAsync(userId, id);
            if(entry != null)
            {
                return View();
            }
            return RedirectToAction("index", "home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Feedback(int id, FeedbackViewModel model)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            UserEntry entry = await userEntryRepository.GetByUserIdAndMeetingIdAsync(userId, id);
            if (entry != null)
            {
                entry.Mark = model.Mark;
                entry.MarkReason = model.MarkReason;
                await userEntryRepository.SaveAsync();

                return View();
            }
            return RedirectToAction("index", "home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUp(int id) //Aanmelden voor een meeting
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Meeting meeting = await meetingRepository.GetByIdAsync(id);
            if (meeting != null) //meeting exists
            {
                UserEntry newEntry = new UserEntry
                {
                    UserId = userId,
                    MeetingId = id
                };
                userEntryRepository.Add(newEntry);
                await userEntryRepository.SaveAsync();
            }
            return RedirectToAction("index");
        }

        [HttpGet]
        public IActionResult Join(int id) //De meeting joinen (videobellen)
        {
            //TODO: Check de role en verwijs gebruiker door naar juiste pagina
            return null;
        }
    }
}
