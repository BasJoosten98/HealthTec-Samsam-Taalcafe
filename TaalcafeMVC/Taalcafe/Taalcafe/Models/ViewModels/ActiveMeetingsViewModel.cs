using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taalcafe.Models.DatabaseModels;

namespace Taalcafe.Models.ViewModels
{
    public class ActiveMeetingsViewModel
    {
        public IEnumerable<ActiveMeetingStats> Stats { get; set; }
    }

    public class ActiveMeetingStats
    {
        public string Participants { get; set; }
        public string JoinUrl { get; set; }
        public string Theme { get; set; }

    }
}
