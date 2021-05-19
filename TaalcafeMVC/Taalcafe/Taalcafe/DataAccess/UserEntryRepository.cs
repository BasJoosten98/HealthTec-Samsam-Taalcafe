using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taalcafe.DbContext;
using Taalcafe.Models.DatabaseModels;

namespace Taalcafe.DataAccess
{
    public class UserEntryRepository : GenericRepository<UserEntry, ApplicationDbContext>
    {
        public UserEntryRepository(ApplicationDbContext context)
            : base(context)
        {

        }

        public async Task<IEnumerable<UserEntry>> GetAllIncludingMeetingAndUser()
        {
            return await Context.UserEntries.Include(entry => entry.Meeting).Include(entry => entry.User).ToListAsync();
        }
        public async Task<IEnumerable<UserEntry>> GetByUserIdAsync(string id)
        {
            return await Context.UserEntries.Where(entry => entry.UserId == id).ToListAsync();
        }

        public async Task<IEnumerable<UserEntry>> GetByMeetingIdAsync(int id)
        {
            return await Context.UserEntries.Where(entry => entry.MeetingId == id).ToListAsync();
        }

        public async Task<IEnumerable<UserEntry>> GetByGroupNumberAsync(string groupNumber)
        {
            return await Context.UserEntries.Where(entry => entry.GroupNumber == groupNumber).ToListAsync();
        }

        public async Task<IEnumerable<UserEntry>> GetByMeetingIdIncludingUserAsync(int id)
        {
            return await Context.UserEntries.Where(entry => entry.MeetingId == id).Include(entry => entry.User).ToListAsync();
        }

        public async Task<IEnumerable<UserEntry>> GetFromTodayByUserIdIncludingMeetingAsync(string id)
        {
            DateTime yesterday = DateTime.Now.AddDays(-1);
            return await Context.UserEntries.Include(entry => entry.Meeting).Where(entry => entry.UserId == id && entry.Meeting.StartDate > yesterday).OrderBy(entry => entry.Meeting.StartDate).ToListAsync();
        }

        public async Task<IEnumerable<UserEntry>> GetThisDayByUserIdIncludingMeetingAsync(string id)
        {
            DateTime yesterday = DateTime.Now.AddDays(-1);
            DateTime tomorrow = DateTime.Now.AddDays(1);
            return await Context.UserEntries.Include(entry => entry.Meeting).Where(entry => entry.UserId == id && entry.Meeting.StartDate > yesterday && entry.Meeting.EndDate < tomorrow).ToListAsync();
        }

        public async Task<UserEntry> GetByUserIdAndMeetingIdAsync(string userId, int meetingId)
        {
            return await Context.UserEntries.Where(entry => entry.UserId == userId && entry.MeetingId == meetingId).FirstOrDefaultAsync();
        }
    }
}
