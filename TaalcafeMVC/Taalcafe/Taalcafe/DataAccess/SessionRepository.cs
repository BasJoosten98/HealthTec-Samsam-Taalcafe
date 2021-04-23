using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taalcafe.DbContext;
using Taalcafe.Models.DatabaseModels;

namespace Taalcafe.DataAccess
{
    public class SessionRepository : GenericRepository<Session, ApplicationDbContext>
    {
        public SessionRepository(ApplicationDbContext context)
            : base(context)
        {

        }

    }
}
