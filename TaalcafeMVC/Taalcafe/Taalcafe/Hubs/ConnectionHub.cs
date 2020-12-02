using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Taalcafe.Models;


namespace Taalcafe.Hubs
{
    public class ConnectionHub : Hub<IConnectionHub>
    {

        private readonly List<UserConnectionInfo> _Users;
        private readonly List<Call> _Calls;
        private readonly List<CallOffer> _CallOffers;

        public ConnectionHub(List<UserConnectionInfo> users, List<Call> calls, List<CallOffer> callOffers)
        {
            _Users = users;
            _Calls = calls;
            _CallOffers = callOffers;
        }

        public async Task Join(string username)
        {
            // Add the new UserConnectionInfo
            var uci = new UserConnectionInfo
            {
                userName = username,
                connectionId = Context.ConnectionId
            };

            _Users.Add(uci);

            // Send the updated list to all clients
            await SendUserListUpdate();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            // Hang up any calls the user is in
            await HangUp(); // Gets the user from "Context" which is available in the whole hub

            // Remove the user
            _Users.RemoveAll(u => u.connectionId == Context.ConnectionId);

            // Send down the new user list to all clients
            await SendUserListUpdate();

            await base.OnDisconnectedAsync(exception);
        }

        public async Task CallUser(UserConnectionInfo targetConnectionId)
        {
            var callingUser = _Users.SingleOrDefault(u => u.connectionId == Context.ConnectionId);
            var targetUser = _Users.SingleOrDefault(u => u.connectionId == targetConnectionId.connectionId);

            // Make sure the person we are trying to call is still here
            if (targetUser == null)
            {
                // If not, let the caller know
                await Clients.Caller.CallDeclined(targetConnectionId, "The user you called has left.");
                return;
            }

            // And that they aren't already in a call
            if (GetUserCall(targetUser.connectionId) != null)
            {
                await Clients.Caller.CallDeclined(targetConnectionId, string.Format("{0} is already in a call.", targetUser.userName));
                return;
            }

            // They are here, so tell them someone wants to talk
            await Clients.Client(targetConnectionId.connectionId).IncomingCall(callingUser);

            // Create an offer
            _CallOffers.Add(new CallOffer(callingUser, targetUser));
        }

        public async Task AnswerCall(bool acceptCall, UserConnectionInfo targetConnectionId)
        {
            var callingUser = _Users.SingleOrDefault(u => u.connectionId == Context.ConnectionId);
            var targetUser = _Users.SingleOrDefault(u => u.connectionId == targetConnectionId.connectionId);

            // This can only happen if the server-side came down and clients were cleared, while the user
            // still held their browser session.
            if (callingUser == null)
            {
                return;
            }

            // Make sure the original caller has not left the page yet
            if (targetUser == null)
            {
                await Clients.Caller.CallEnded(targetConnectionId, "The other user in your call has left.");
                return;
            }

            // Send a decline message if the callee said no
            if (acceptCall == false)
            {
                await Clients.Client(targetConnectionId.connectionId).CallDeclined(callingUser, string.Format("{0} did not accept your call.", callingUser.userName));
                return;
            }

            // Make sure there is still an active offer.  If there isn't, then the other use hung up before the Callee answered.
            var offerCount = _CallOffers.RemoveAll(c => c.Callee.connectionId == callingUser.connectionId
                                                  && c.Caller.connectionId == targetUser.connectionId);
            if (offerCount < 1)
            {
                await Clients.Caller.CallEnded(targetConnectionId, string.Format("{0} has already hung up.", targetUser.userName));
                return;
            }

            // And finally... make sure the user hasn't accepted another call already
            if (GetUserCall(targetUser.connectionId) != null)
            {
                // And that they aren't already in a call
                await Clients.Caller.CallDeclined(targetConnectionId, string.Format("{0} chose to accept someone elses call instead of yours :(", targetUser.userName));
                return;
            }

            // Remove all the other offers for the call initiator, in case they have multiple calls out
            _CallOffers.RemoveAll(c => c.Caller.connectionId == targetUser.connectionId);

            // Create a new call to match these users up
            _Calls.Add(new Call(new List<UserConnectionInfo> { callingUser, targetUser }));

            // Tell the original caller that the call was accepted
            await Clients.Client(targetConnectionId.connectionId).CallAccepted(callingUser);

            // Update the user list, since these two are now in a call
            await SendUserListUpdate();
        }

        public async Task HangUp()
        {
            var callingUser = _Users.SingleOrDefault(u => u.connectionId == Context.ConnectionId);

            if (callingUser == null)
            {
                return;
            }

            var currentCall = GetUserCall(callingUser.connectionId);

            // Send a hang up message to each user in the call, if there is one
            if (currentCall != null)
            {
                foreach (var user in currentCall.Users.Where(u => u.connectionId != callingUser.connectionId))
                {
                    await Clients.Client(user.connectionId).CallEnded(callingUser, string.Format("{0} has hung up.", callingUser.userName));
                }

                // Remove the call from the list if there is only one (or none) person left.  This should
                // always trigger now, but will be useful when we implement conferencing.
                currentCall.Users.RemoveAll(u => u.connectionId == callingUser.connectionId);
                if (currentCall.Users.Count < 2)
                {
                    _Calls.Remove(currentCall);
                }
            }

            // Remove all offers initiating from the caller
            _CallOffers.RemoveAll(c => c.Caller.connectionId == callingUser.connectionId);

            await SendUserListUpdate();
        }

        // WebRTC Signal Handler
        public async Task SendSignal(string signal, string targetConnectionId)
        {
            var callingUser = _Users.SingleOrDefault(u => u.connectionId == Context.ConnectionId);
            var targetUser = _Users.SingleOrDefault(u => u.connectionId == targetConnectionId);

            // Make sure both users are valid
            if (callingUser == null || targetUser == null)
            {
                return;
            }

            // Make sure that the person sending the signal is in a call
            var userCall = GetUserCall(callingUser.connectionId);

            // ...and that the target is the one they are in a call with
            if (userCall != null && userCall.Users.Exists(u => u.connectionId == targetUser.connectionId))
            {
                // These folks are in a call together, let's let em talk WebRTC
                await Clients.Client(targetConnectionId).ReceiveSignal(callingUser, signal);
            }
        }

        private async Task SendUserListUpdate()
        {
            _Users.ForEach(u => u.inCall = (GetUserCall(u.connectionId) != null));
            await Clients.All.UpdateUserList(_Users);
        }

        private Call GetUserCall(string connectionId)
        {
            var matchingCall =
                _Calls.SingleOrDefault(uc => uc.Users.SingleOrDefault(u => u.connectionId == connectionId) != null);
            return matchingCall;
        }
    }
}