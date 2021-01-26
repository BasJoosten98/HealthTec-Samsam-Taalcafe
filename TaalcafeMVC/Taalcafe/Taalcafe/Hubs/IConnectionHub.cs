using System.Collections.Generic;
using System.Threading.Tasks;
using Taalcafe.Models;

namespace Taalcafe.Hubs
{
    public interface IConnectionHub
    {
        Task UpdateUserList(List<UserConnectionInfo> userList);
        Task UpdateActiveCalls(List<Call> callList);
        Task CallAccepted(UserConnectionInfo acceptingUser);
        Task CallDeclined(UserConnectionInfo decliningUser, string reason);
        Task IncomingCall(UserConnectionInfo callingUser);
        Task ReceiveSignal(UserConnectionInfo signalingUser, string signal);
        Task UserLeft(UserConnectionInfo leavingUser);
        Task CallEnded(UserConnectionInfo signalingUser, string signal);
    }
}