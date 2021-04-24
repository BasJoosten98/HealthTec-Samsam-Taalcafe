using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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

        public MeetingController(MeetingRepository meetingRepository,
            UserEntryRepository userEntryRepository)
        {
            this.meetingRepository = meetingRepository;
            this.userEntryRepository = userEntryRepository;
        }

        public async Task<ActionResult> Index()
        {
            IEnumerable<Meeting> model = await meetingRepository.GetAllAsync();
            return View(model);
        }

        public async Task<ActionResult> Details(int id)
        {
            Meeting model = await meetingRepository.GetByIdAsync(id);
            return View(model);
        }

        public ActionResult Create()
        {
            Meeting model = new Meeting();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Meeting createdMeeting)
        {
            if (ModelState.IsValid)
            {
                meetingRepository.Add(createdMeeting);
                await meetingRepository.SaveAsync();

                return RedirectToAction("details", new { id = createdMeeting.Id });
            }
            else
            {
                return View();
            }
        }

        public async Task<ActionResult> Edit(int id)
        {
            Meeting model = await meetingRepository.GetByIdAsync(id);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, Meeting updatedMeeting)
        {
            if (ModelState.IsValid)
            {
                await meetingRepository.SaveAsync();

                return RedirectToAction("details", new { id = updatedMeeting.Id });
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
            meetingRepository.Remove(meeting);
            await meetingRepository.SaveAsync();
            return RedirectToAction("index");
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
