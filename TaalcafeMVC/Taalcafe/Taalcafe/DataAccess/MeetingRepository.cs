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

    }
}
