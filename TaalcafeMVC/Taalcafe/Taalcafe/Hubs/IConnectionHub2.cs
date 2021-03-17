using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taalcafe.Hubs.HubModels;

namespace Taalcafe.Hubs
{
    public interface IConnectionHub2
    {
        Task JoinedSuccess();
        Task JoinedFailed(string reason);
        Task ReceiveOnlineGroups(List<OnlineGroup> groups);
        Task NeedHelpSetTo(bool val);
        Task CallUser(OnlineUser user);
        Task UserHasLeft(OnlineUser user);
        Task ReceiveSignal(OnlineUser user, string data);

    }
}
