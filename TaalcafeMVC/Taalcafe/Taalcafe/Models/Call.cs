using System.Collections.Generic;

namespace Taalcafe.Models
{
    public class Call
    {
        public List<UserConnectionInfo> Users { get; set; }
        public bool help { get; set; }

        public Call(List<UserConnectionInfo> users, bool help = false) 
        {
            Users = users;
            this.help = help;
        }
    }
}