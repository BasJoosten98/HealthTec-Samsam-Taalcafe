using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taalcafe.DbContext;
using Taalcafe.Models.DatabaseModels;

namespace Taalcafe.DataAccess
{
    public class MeetingRepository : GenericRepository<Meeting, ApplicationDbContext>
    {
        public MeetingRepository(ApplicationDbContext context)
            : base(context)
        {

        }

        public async Task<IEnumerable<Meeting>> GetAllMeetingsIncludingThemes()
        {
            return await Context.Meetings.Include(meeting => meeting.Theme).OrderBy(meeting => meeting.StartDate).ToListAsync();
        }

        public async Task<IEnumerable<Meeting>> GetAllUpcomingMeetingsIncludingThemes()
        {
            return await Context.Meetings.Where(meeting => meeting.StartDate >= DateTime.Now).Include(meeting => meeting.Theme).OrderBy(meeting => meeting.StartDate).ToListAsync();
        }

        public async Task<IEnumerable<Meeting>> GetAllPastMeetingsIncludingThemes()
        {
            return await Context.Meetings.Where(meeting => meeting.StartDate < DateTime.Now).Include(meeting => meeting.Theme).OrderBy(meeting => meeting.StartDate).ToListAsync();
        }

    }
}
