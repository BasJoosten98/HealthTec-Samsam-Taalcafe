using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using Microsoft.Graph.Auth;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using Taalcafe.DataAccess;
using Taalcafe.Models.DatabaseModels;
using Taalcafe.Models.ViewModels;

namespace Taalcafe.Controllers
{
    [Authorize(Roles = "Coordinator, Cursist, Taalcoach")]
    public class MeetingController : Controller
    {
        private readonly MeetingRepository meetingRepository;
        private readonly UserEntryRepository userEntryRepository;
        private readonly ThemeRepository themeRepository;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IConfiguration config;

        public MeetingController(MeetingRepository meetingRepository,
            UserEntryRepository userEntryRepository,
            ThemeRepository themeRepository,
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration)
        {
            this.meetingRepository = meetingRepository;
            this.userEntryRepository = userEntryRepository;
            this.themeRepository = themeRepository;
            this.userManager = userManager;
            this.config = configuration;
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

        [Authorize(Roles = "Coordinator")]
        public async Task<ActionResult> Create()
        {
            IEnumerable<Theme> themes = await themeRepository.GetAllAsync();
            if(themes.Count() <= 0) //no themes exist
            {
                TempData["title"] = "Geen thema's";
                List<string> content = new List<string>();
                content.Add("Een meeting heeft een thema nodig om gemaakt te worden.");
                content.Add("Er bestaan echter nog geen thema's.");
                content.Add("Deze zullen dus als eerst gemaakt moeten worden.");
                TempData["content"] = content;
                TempData["action"] = "index";
                TempData["controller"] = "theme";

                return RedirectToAction("message", "home");
            }

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
        [Authorize(Roles = "Coordinator")]
        public async Task<ActionResult> Create(CreateMeetingViewModel model)
        {
            if(model.StartDate >= model.EndDate)
            {
                ModelState.AddModelError("EndDate", "Eindtijd moet later zijn dan starttijd");
            }
            else if (ModelState.IsValid)
            {
                              
                Meeting meeting = new Meeting
                {
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    ThemeId = model.ThemeId
                };
                meetingRepository.Add(meeting);
                await meetingRepository.SaveAsync();

                TempData["title"] = "Meeting toegevoegd!";
                List<string> content = new List<string>();
                content.Add("De meeting is toegevoegd. ");
                TempData["content"] = content;
                TempData["action"] = "index";
                TempData["controller"] = "meeting";

                return RedirectToAction("message", "home");           
            }
            IEnumerable<Theme> themes = await themeRepository.GetAllAsync();
            model.ThemeSelectList = themes.Select(theme => new SelectListItem(theme.Title, theme.Id.ToString()));
            return View(model);
        }

        [Authorize(Roles = "Coordinator")]
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
        [Authorize(Roles = "Coordinator")]
        public async Task<ActionResult> Edit(int id, CreateMeetingViewModel model)
        {
            if (model.StartDate >= model.EndDate)
            {
                ModelState.AddModelError("EndDate", "Eindtijd moet later zijn dan starttijd");
            }
            else if (ModelState.IsValid)
            {
                //model.Id = id;
                Meeting meeting = new Meeting
                {
                    Id = id,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    ThemeId = model.ThemeId
                };
                meetingRepository.Update(meeting);
                await meetingRepository.SaveAsync();

                TempData["title"] = "Meeting bijgewerkt!";
                List<string> content = new List<string>();
                content.Add("De meeting is bijgewerkt. ");
                TempData["content"] = content;
                TempData["action"] = "index";
                TempData["controller"] = "meeting";

                return RedirectToAction("message", "home");
            }
            IEnumerable<Theme> themes = await themeRepository.GetAllAsync();
            model.ThemeSelectList = themes.Select(theme => new SelectListItem(theme.Title, theme.Id.ToString()));
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Coordinator")]
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

        //[HttpGet]
        //public async Task<IActionResult> Feedback(int id)
        //{
        //    string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //    UserEntry entry = await userEntryRepository.GetByUserIdAndMeetingIdAsync(userId, id);
        //    if(entry != null)
        //    {
        //        return View();
        //    }
        //    return RedirectToAction("index", "home");
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Feedback(int id, FeedbackViewModel model)
        //{
        //    string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //    UserEntry entry = await userEntryRepository.GetByUserIdAndMeetingIdAsync(userId, id);
        //    if (entry != null)
        //    {
        //        entry.Mark = model.Mark;
        //        entry.MarkReason = model.MarkReason;
        //        await userEntryRepository.SaveAsync();

        //        return View();
        //    }
        //    return RedirectToAction("index", "home");
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Cursist, Taalcoach")]
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
                content.Add($"De meeting vind plaats van {meeting.StartDate.ToString("ddd d MMMM yyyy, H:mm")} tot en met {meeting.EndDate.ToString("ddd d MMMM yyyy, H:mm")}.");
                content.Add("");
                content.Add("Tot dan!");
                TempData["content"] = content;
                //TempData["action"] = "index";
                //TempData["controller"] = "meeting";

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Cursist, Taalcoach")]
        public async Task<IActionResult> SignOut(int id) //Afmelden voor een meeting
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            UserEntry entry = await userEntryRepository.GetByUserIdAndMeetingIdAsync(userId, id);
            if (entry != null && !string.IsNullOrEmpty(userId)) //meeting exists
            {
                userEntryRepository.Remove(entry);
                await userEntryRepository.SaveAsync();

                TempData["title"] = "Afgemeld!";
                List<string> content = new List<string>();
                content.Add("U bent afgemeld voor de meeting!");
                TempData["content"] = content;

                return RedirectToAction("message", "home");
            }
            TempData["title"] = "Mislukt!";
            List<string> content2 = new List<string>();
            content2.Add("U bent NIET afgemeld voor de meeting!");
            content2.Add("Iets ging fout bij ons...");
            content2.Add("Mogelijk is de meeting net verwijderd of zal u zich opnieuw moeten inloggen.");
            content2.Add("Sorry voor het ongemak.");
            TempData["content"] = content2;
            TempData["action"] = "index";
            TempData["controller"] = "meeting";

            return RedirectToAction("message", "home");
        }

        [HttpGet]
        [Authorize(Roles = "Coordinator")]
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

        private class GroupCountersHolder
        {
            public List<GroupCounter> Counters { get; set; }
            public List<UserEntry> Entries { get; set; }
        }
        private class GroupCounter
        {
            public string GroupName { get; set; }
            public List<ManageGroupsUserModel> Users { get; set; }
            public List<ManageGroupsUserModel> ParticipatedUsers { get; set; }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Coordinator")]
        public async Task<IActionResult> ManageGroups(int id, string Users)
        {
            List<ManageGroupsUserModel> userList = JsonConvert.DeserializeObject<List<ManageGroupsUserModel>>(Users); //new groups
            //Dictionary<string, string> groupIds = new Dictionary<string, string>();
            IEnumerable<UserEntry> entries = await userEntryRepository.GetByMeetingIdAsync(id); //old groups from database
            List<string> usersWhoNeedToRecall = new List<string>(); //users that need to rejoin the video call
            List<GroupCountersHolder> counterHolder = new List<GroupCountersHolder>();

            while(userList.Count > 0)
            {
                ManageGroupsUserModel user = userList[0];
                UserEntry userEntry = entries.Where(e => e.UserId == user.UserId).FirstOrDefault();
                if(userEntry != null)
                {
                    if(string.IsNullOrEmpty(user.GroupName))
                    {
                        userEntry.GroupNumber = null;
                        userEntry.Joined_at = null;
                        userEntryRepository.Update(userEntry);
                        userList.RemoveAt(0);
                        continue;
                    }
                    else
                    {
                        List<ManageGroupsUserModel> groupUsers = userList.Where(e => e.GroupName == user.GroupName).ToList();
                        List<GroupCounter> groupCounters = new List<GroupCounter>();
                        List<UserEntry> groupEntries = new List<UserEntry>();
                        GroupCountersHolder holder = new GroupCountersHolder();

                        //Create GroupCounters for gathering all groupdata
                        foreach(ManageGroupsUserModel groupUser in groupUsers)
                        {
                            UserEntry groupUserEntry = entries.Where(e => e.UserId == groupUser.UserId).FirstOrDefault();
                            if(groupUserEntry != null)
                            {
                                groupEntries.Add(groupUserEntry);
                                if (!string.IsNullOrEmpty(groupUserEntry.GroupNumber))
                                {
                                    GroupCounter counter = groupCounters.Where(g => g.GroupName == groupUserEntry.GroupNumber).FirstOrDefault();
                                    if (counter == null)
                                    {
                                        counter = new GroupCounter
                                        {
                                            GroupName = groupUserEntry.GroupNumber,
                                            Users = new List<ManageGroupsUserModel>() { groupUser },
                                            ParticipatedUsers = (groupUserEntry.Joined_at != null) ? new List<ManageGroupsUserModel>() { groupUser } : new List<ManageGroupsUserModel>()
                                        };
                                        groupCounters.Add(counter);
                                    }
                                    else
                                    {
                                        counter.Users.Add(groupUser);
                                        if (groupUserEntry.Joined_at != null) { counter.ParticipatedUsers.Add(groupUser); }
                                    }
                                }
                            }
                            userList.Remove(groupUser);
                        }
                        holder.Counters = groupCounters;
                        holder.Entries = groupEntries;
                        counterHolder.Add(holder);                      
                    }
                }
            }

            //Order counters in every holder such that most participated is first
            counterHolder.ForEach(h => h.Counters = h.Counters.OrderByDescending(c => c.ParticipatedUsers.Count).ToList());

            while (counterHolder.Count > 0)
            {
                //Give groups with no counter a random GUID as groupname
                foreach (GroupCountersHolder holder in counterHolder)
                {
                    if (holder.Counters.Count == 0)
                    {
                        string newGroupName = Guid.NewGuid().ToString();
                        holder.Entries.ForEach(e => {
                            e.GroupNumber = newGroupName;
                            e.Joined_at = null;
                            userEntryRepository.Update(e);
                        });
                    }
                }

                //Filter out the groups with no counter and order them
                counterHolder = counterHolder.Where(h => h.Counters.Count > 0).OrderByDescending(h => h.Counters[0].ParticipatedUsers.Count).ToList();

                if(counterHolder.Count > 0)
                {
                    //Give the curHolder the best counter groupname it has
                    GroupCountersHolder curHolder = counterHolder[0];
                    string bestGroupName = curHolder.Counters[0].GroupName;
                    foreach (UserEntry entry in curHolder.Entries)
                    {
                        if(entry.GroupNumber != bestGroupName) { entry.Joined_at = null; }
                        entry.GroupNumber = bestGroupName;
                        userEntryRepository.Update(entry);
                    }

                    //Add recall participants && remove counters with the same groupname
                    for (int i = 1; i < curHolder.Counters.Count; i++) //curholder
                    {
                        curHolder.Counters[i].ParticipatedUsers.ForEach(user => usersWhoNeedToRecall.Add(user.UserName));
                    }
                    for (int i = 1; i < counterHolder.Count; i++) //other holders
                    {
                        GroupCountersHolder holder = counterHolder[i];
                        foreach(GroupCounter counter in holder.Counters)
                        {
                            if(counter.GroupName == bestGroupName)
                            {
                                counter.ParticipatedUsers.ForEach(user => usersWhoNeedToRecall.Add(user.UserName));
                                holder.Counters.Remove(counter);
                                break;
                            }
                        }
                    }

                    //remove curholder
                    counterHolder.RemoveAt(0);
                }
            }

            if (userEntryRepository.HasChanges())
            {
                await userEntryRepository.SaveAsync();
            }

            TempData["title"] = "Groepen opgeslagen!";
            List<string> content = new List<string>();
            content.Add("De door u gemaakte groepen voor deze meeting zijn opgeslagen.");
            if(usersWhoNeedToRecall.Count > 0)
            {
                content.Add("");
                content.Add("Echter zijn sommige gebruikers al aan het videobellen!");
                content.Add("De volgende gebruikers moet gevraagd worden om opnieuw te gaan videobellen:");
                foreach(string name in usersWhoNeedToRecall)
                {
                    content.Add("- " + name);
                }
                content.Add("");
            }
            TempData["content"] = content;
            TempData["action"] = "index";
            TempData["controller"] = "meeting";

            return RedirectToAction("message", "home");
        }

        private class activeMeetingInfo
        {
            public string Participants { get; set; }
            public string HasParticipated { get; set; }
            public string Theme { get; set; }
        }

        [HttpGet]
        [Authorize(Roles = "Coordinator")]
        public async Task<IActionResult> ActiveMeetings()
        {
            IEnumerable<UserEntry> entries = await userEntryRepository.GetAllIncludingMeetingAndUser();
            entries = entries.Where(entry => entry.Meeting.StartDate <= DateTime.Now && entry.Meeting.EndDate > DateTime.Now).OrderBy(e => e.Joined_at);
            List<ActiveMeetingStats> stats = new List<ActiveMeetingStats>();
            Dictionary<string, activeMeetingInfo> joinAndParticipants = new Dictionary<string, activeMeetingInfo>();

            foreach(UserEntry entry in entries)
            {
                if (!string.IsNullOrEmpty(entry.GroupNumber))
                {
                    if (entry.GroupNumber.ToLower().Contains("teams.microsoft.com"))
                    {
                        if (joinAndParticipants.ContainsKey(entry.GroupNumber))
                        {
                            joinAndParticipants[entry.GroupNumber].Participants += ", " + entry.User.UserName;
                        }
                        else
                        {
                            joinAndParticipants[entry.GroupNumber] = new activeMeetingInfo { Participants = entry.User.UserName, Theme = entry.Meeting.Theme.Title };
                        }

                        if (entry.Joined_at != null)
                        {
                            if (string.IsNullOrEmpty(joinAndParticipants[entry.GroupNumber].HasParticipated))
                            {
                                joinAndParticipants[entry.GroupNumber].HasParticipated = entry.User.UserName;
                            }
                            else
                            {
                                joinAndParticipants[entry.GroupNumber].HasParticipated += ", " + entry.User.UserName;
                            }
                        }
                    }
                }
            }

            foreach(var item in joinAndParticipants)
            {
                ActiveMeetingStats temp = new ActiveMeetingStats
                {
                    Participants = item.Value.Participants,
                    HasParticipated = item.Value.HasParticipated,
                    JoinUrl = item.Key,
                    Theme = item.Value.Theme
                };
                stats.Add(temp);
            }
            ActiveMeetingsViewModel model = new ActiveMeetingsViewModel { Stats = stats };
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "Cursist, Taalcoach")]
        public async Task<IActionResult> Join() //De meeting joinen (videobellen)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            IEnumerable<UserEntry> entries = await userEntryRepository.GetThisDayByUserIdIncludingMeetingAsync(userId);
            UserEntry entry = entries.Where(entry => entry.Meeting.StartDate <= DateTime.Now && entry.Meeting.EndDate > DateTime.Now).FirstOrDefault();

            if (entry != null)
            {
                if(string.IsNullOrEmpty(entry.GroupNumber))
                {
                    TempData["title"] = "Geen groep";
                    List<string> content2 = new List<string>();
                    content2.Add("Op dit moment is er meeting bezig waarvoor u zich heeft aangemeld.");
                    content2.Add("Echter bent u niet ingedeeld in een groep door een van de coordinatoren.");
                    content2.Add("Neem contact op met een van de coordinatoren om dit probleem op te lossen.");
                    TempData["content"] = content2;

                    return RedirectToAction("message", "home");
                }
                if (entry.GroupNumber.ToLower().Contains("teams.microsoft.com")) //join url has been added already
                {
                    entry.Joined_at = DateTime.Now;
                    userEntryRepository.Update(entry);
                    await userEntryRepository.SaveAsync();
                    TempData["joinUrl"] = entry.GroupNumber;
                    return View();
                }

                //Meeting meeting = await meetingRepository.GetByIdAsync(entry.MeetingId);
                OnlineMeeting result = await createTeamsMeetingAsync(entry.Meeting.StartDate, entry.Meeting.EndDate);

                //updating group number to join URL
                IEnumerable<UserEntry> group = await userEntryRepository.GetByGroupNumberAsync(entry.GroupNumber);
                foreach(UserEntry eu in group)
                {
                    eu.GroupNumber = result.JoinWebUrl;
                    userEntryRepository.Update(eu);
                }

                entry.Joined_at = DateTime.Now;
                userEntryRepository.Update(entry);
                await userEntryRepository.SaveAsync();

                TempData["joinUrl"] = result.JoinWebUrl;
                return View();
            }
            TempData["title"] = "Geen meeting nu";
            List<string> content = new List<string>();
            content.Add("Op dit moment is er geen meeting bezig waarvoor u zich heeft aangemeld.");
            content.Add("Probeer het later opnieuw.");
            TempData["content"] = content;

            return RedirectToAction("message", "home");
        }

        [NonAction]
        public async Task<OnlineMeeting> createTeamsMeetingAsync(DateTime start, DateTime end)
        {
            var confidentialClient = ConfidentialClientApplicationBuilder
                               .Create(config["MeetingConfig:App-Id"])
                               .WithTenantId(config["MeetingConfig:Tenant-Id"])
                               .WithClientSecret(config["MeetingConfig:Client-Secret"])
                               .Build();

            var authProvider = new ClientCredentialProvider(confidentialClient);

            var graphClient = new GraphServiceClient(authProvider);

            LobbyBypassSettings lobbySettings = new LobbyBypassSettings();
            lobbySettings.Scope = LobbyBypassScope.Everyone;
            lobbySettings.IsDialInBypassEnabled = true;
            var onlineMeeting = new OnlineMeeting
            {
                StartDateTime = start.ToUniversalTime(),
                EndDateTime = end.ToUniversalTime(),
                Subject = "Samsam Taalcafe Meeting",
                LobbyBypassSettings = lobbySettings
            };

            var result = await graphClient.Users[config["MeetingConfig:User-Id"]].OnlineMeetings
                .Request()
                .AddAsync(onlineMeeting);

            return result;
        }
    }

    
}
