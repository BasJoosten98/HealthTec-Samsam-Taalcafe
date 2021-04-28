using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
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
        private readonly UserManager<ApplicationUser> userManager;

        public MeetingController(MeetingRepository meetingRepository,
            UserEntryRepository userEntryRepository,
            ThemeRepository themeRepository,
            UserManager<ApplicationUser> userManager)
        {
            this.meetingRepository = meetingRepository;
            this.userEntryRepository = userEntryRepository;
            this.themeRepository = themeRepository;
            this.userManager = userManager;
        }

        public async Task<ActionResult> Index()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            IEnumerable<Meeting> meetings = await meetingRepository.GetAllMeetingsIncludingThemes();
            IEnumerable<Meeting> past = meetings.Where(meeting => meeting.EndDate < DateTime.Now);
            IEnumerable<Meeting> upcoming = meetings.Where(meeting => meeting.EndDate >= DateTime.Now);
            IEnumerable<UserEntry> entries = await userEntryRepository.GetByUserIdAsync(userId);

            IEnumerable<Meeting> signedUp = upcoming.Where(meeting => entries.Any(entry => entry.MeetingId == meeting.Id));
            upcoming = upcoming.Where(meeting => !entries.Any(entry => entry.MeetingId == meeting.Id));

            MeetingIndexViewModel model = new MeetingIndexViewModel
            {
                PastMeetings = past,
                UpcomingMeetings = upcoming,
                SignedUpMeetings = signedUp
            };

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
            if (meeting != null && !string.IsNullOrEmpty(userId)) //meeting exists
            {
                UserEntry newEntry = new UserEntry
                {
                    UserId = userId,
                    MeetingId = id
                };
                userEntryRepository.Add(newEntry);
                await userEntryRepository.SaveAsync();

                TempData["title"] = "Aangemeld!";
                List<string> content = new List<string>();
                content.Add("U bent aangemeld voor de meeting!");
                content.Add($"De meeting vind plaats van {meeting.StartDate.ToString("dd-MM-yyyy HH:mm")} tot en met {meeting.EndDate.ToString("dd-MM-yyyy HH:mm")}.");
                content.Add("");
                content.Add("Tot dan!");
                TempData["content"] = content;
                TempData["action"] = "index";
                TempData["controller"] = "meeting";

                return RedirectToAction("message", "home");
            }
            TempData["title"] = "Mislukt!";
            List<string> content2 = new List<string>();
            content2.Add("U bent NIET aangemeld voor de meeting!");
            content2.Add("Iets ging fout bij ons...");
            content2.Add("Mogelijk is de meeting net verwijderd of zal u zich opnieuw moeten inloggen.");
            content2.Add("Sorry voor het ongemak.");
            TempData["content"] = content2;
            TempData["action"] = "index";
            TempData["controller"] = "meeting";

            return RedirectToAction("message", "home");
        }

        [HttpGet]
        public async Task<IActionResult> ManageGroups(int id)
        {
            IEnumerable<UserEntry> AllUsers = await userEntryRepository.GetByMeetingIdIncludingUserAsync(id);
            List<ManageGroupsUserModel> UserModels = new List<ManageGroupsUserModel>();
            foreach(UserEntry entry in AllUsers)
            {
                ManageGroupsUserModel model = new ManageGroupsUserModel
                {
                    UserId = entry.UserId,
                    UserName = entry.User.UserName,
                    GroupName = entry.GroupNumber
                };
                ApplicationUser user = await userManager.FindByIdAsync(entry.UserId);
                IEnumerable<string> roles = await userManager.GetRolesAsync(user);
                model.Role = roles.First();
                UserModels.Add(model);
            }
            return View(new ManageGroupsViewModel { Users = UserModels });

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageGroups(int id, string Users)
        {
            ManageGroupsUserModel[] userList = JsonConvert.DeserializeObject<ManageGroupsUserModel[]>(Users);
            Dictionary<string, string> groupIds = new Dictionary<string, string>();
            IEnumerable<UserEntry> entries = await userEntryRepository.GetByMeetingIdAsync(id);
            foreach (ManageGroupsUserModel user in userList)
            {
                UserEntry entry = entries.Where(entry => entry.UserId == user.UserId).FirstOrDefault();
                if (user.GroupName != null)
                {
                    if (!groupIds.ContainsKey(user.GroupName))
                    {
                        groupIds.Add(user.GroupName, Guid.NewGuid().ToString());
                    }
                    entry.GroupNumber = groupIds[user.GroupName];
                    userEntryRepository.Update(entry);
                    continue;
                }
                entry.GroupNumber = null;
                userEntryRepository.Update(entry);
            }
            await userEntryRepository.SaveAsync();

            TempData["title"] = "Groepen opgeslagen!";
            List<string> content = new List<string>();
            content.Add("De door u gemaakte groepen voor deze meeting zijn opgeslagen.");
            TempData["content"] = content;
            TempData["action"] = "index";
            TempData["controller"] = "meeting";

            return RedirectToAction("message", "home");
        }

        [HttpGet]
        public IActionResult Join(int id) //De meeting joinen (videobellen)
        {
            //TODO: Check de role en verwijs gebruiker door naar juiste pagina
            return null;
        }
    }
}
