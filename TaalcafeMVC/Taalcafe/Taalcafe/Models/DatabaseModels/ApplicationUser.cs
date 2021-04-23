using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Taalcafe.Models.DatabaseModels
{
    public class ApplicationUser
    {
        public virtual ICollection<UserEntry> UserEntries { get; set; }
    }
}
