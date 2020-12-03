using System.Collections.Generic;

namespace Taalcafe.Models
{
    public class Call
    {
        public List<UserConnectionInfo> Users { get; set; }

        public Call(List<UserConnectionInfo> users) 
        {
            Users = users;
        }
    }
}