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

        public async Task<IEnumerable<UserEntry>> GetByUserIdAsync(string id)
        {
            return await Context.UserEntries.Where(entry => entry.UserId == id).ToListAsync();
        }

        public async Task<IEnumerable<UserEntry>> GetByMeetingIdAsync(int id)
        {
            return await Context.UserEntries.Where(entry => entry.MeetingId == id).ToListAsync();
        }

        public async Task<IEnumerable<UserEntry>> GetByMeetingIdIncludingUserAsync(int id)
        {
            return await Context.UserEntries.Where(entry => entry.MeetingId == id).Include(entry => entry.User).ToListAsync();
        }

        public async Task<UserEntry> GetByUserIdAndMeetingIdAsync(string userId, int meetingId)
        {
            return await Context.UserEntries.Where(entry => entry.UserId == userId && entry.MeetingId == meetingId).FirstOrDefaultAsync();
        }
    }
}
