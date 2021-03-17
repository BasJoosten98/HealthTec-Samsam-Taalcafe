using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Taalcafe.Hubs.HubModels
{
    public class OnlineGroup
    {
        public OnlineGroup()
        {
            OnlineUsers = new List<OnlineUser>();
        }
        public int GroupId { get; set; }
        public List<OnlineUser> OnlineUsers { get; set; }
        public bool NeedsHelp { get; set; }
        public OnlineUser Coordinator { get; set; }
    }
}
