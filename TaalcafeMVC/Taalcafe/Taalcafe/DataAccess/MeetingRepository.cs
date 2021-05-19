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

        public async Task<Meeting> GetByIdIncludingThemes(int id)
        {
            return await Context.Meetings.Include(meeting => meeting.Theme).Where(meeting => meeting.Id == id).OrderBy(meeting => meeting.StartDate).FirstOrDefaultAsync();
        }
    }
}
