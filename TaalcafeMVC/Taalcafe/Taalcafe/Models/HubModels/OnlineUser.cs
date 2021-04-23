using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Taalcafe.Models.HubModels
{
    public class OnlineUser
    {
        public string Name { get; set; }
        public int UserId { get; set; }
        public string ConnectionId { get; set; }
    }
}
