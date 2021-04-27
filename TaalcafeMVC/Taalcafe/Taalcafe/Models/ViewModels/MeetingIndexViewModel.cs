using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taalcafe.Models.DatabaseModels;

namespace Taalcafe.Models.ViewModels
{
    public class MeetingIndexViewModel
    {
        public IEnumerable<Meeting> SignedUpMeetings { get; set; }

        public IEnumerable<Meeting> UpcomingMeetings { get; set; }

        public IEnumerable<Meeting> PastMeetings { get; set; }
    }
}
