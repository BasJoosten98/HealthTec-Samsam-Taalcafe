using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taalcafe.Hubs.HubModels;
using Taalcafe.Models.DB;
using Taalcafe.Models.HubModels;

namespace Taalcafe.Hubs
{
    public class ConnectionHub2 : Hub<IConnectionHub2>
    {
        private readonly List<OnlineGroup> onlineGroups;
        private readonly List<OnlineUser> onlineCoordinators;
        private readonly dbi380705_taalcafeContext dbContext;

        public ConnectionHub2(List<OnlineUser> coordinators, List<OnlineGroup> groups)
        {
            onlineCoordinators = coordinators;
            onlineGroups = groups;
            dbContext = new dbi380705_taalcafeContext();
        }


        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Leave();
            OnlineUser coordinator = onlineCoordinators.SingleOrDefault(c => c.ConnectionId == Context.ConnectionId);
            if (coordinator != null) //user is a coordinator, so log him out!
            {
                onlineCoordinators.RemoveAll(c => c.ConnectionId == Context.ConnectionId || c.UserId == coordinator.UserId);
            }
        }
        //Look up group and let coordinators know that this group needs help (in call overview)
        public async Task AskForHelp()
        {
            OnlineGroup group = onlineGroups.SingleOrDefault(group => group.OnlineUsers.SingleOrDefault(user => user.ConnectionId == Context.ConnectionId) != null);
            if (group != null)
            {
                group.NeedsHelp = !group.NeedsHelp;
                foreach(OnlineUser user in group.OnlineUsers)
                {
                    await Clients.Client(user.ConnectionId).NeedHelpSetTo(group.NeedsHelp); 
                }
                //Coordinator can not see the help symbol in call
                //if(group.Coordinator != null)
                //{
                //    await Clients.Client(group.Coordinator.ConnectionId).NeedHelpSetTo(group.NeedsHelp);
                //}
                await UpdateOnlineGroups();
            }
            
        }

        //Check if user is allowed to join, if yes (set group as online) and partners are online order him to call partners
        public async Task Join(int userId)
        {

            var coordinator = dbContext.Gebruikers.Include(g => g.Account).SingleOrDefault(g => g.Id == userId);
            if(coordinator != null)
            {
                if(coordinator.Account.Type == "Coordinator")
                {
                    OnlineUser user = new OnlineUser();
                    user.Name = coordinator.Naam;
                    user.UserId = coordinator.Id;
                    user.ConnectionId = Context.ConnectionId;

                    OnlineUser c = onlineCoordinators.SingleOrDefault(c => c.ConnectionId == Context.ConnectionId);
                    if(c != null) { 
                        //await Clients.Caller.JoinedFailed("This coordinator has already joined (connectionId)");
                        await Clients.Caller.ServerMessage(1, "This coordinator has already joined (connectionId)");
                        return; 
                    }
                    c = onlineCoordinators.SingleOrDefault(c => c.UserId == user.UserId);
                    if (c != null) { 
                        //await Clients.Caller.JoinedFailed("This coordinator has already joined (userId)");
                        await Clients.Caller.ServerMessage(1, "This coordinator has already joined (userId)");
                        return; 
                    }

                    onlineCoordinators.Add(user);
                    //await Clients.Caller.JoinedSuccess();
                    await Clients.Caller.ServerMessage(2, "You joined as a coordinator");
                    await Clients.Caller.ReceiveOnlineGroups(onlineGroups);
                    return;
                }
            }

            SessiePartner sessiePartner = dbContext.SessiePartners
                .Include(c => c.Taalcoach)
                .Include(c => c.Cursist)
                .Include(c => c.Sessie)
                    .ThenInclude(s => s.Thema)
                .SingleOrDefault(c => c.Sessie.Datum.Value.Date == DateTime.Today && (c.TaalcoachId == userId || c.CursistId == userId));
            if (sessiePartner != null)
            {
                OnlineUser user = new OnlineUser();
                Gebruiker g = (sessiePartner.CursistId == userId) ? sessiePartner.Cursist : sessiePartner.Taalcoach;
                user.Name = g.Naam;
                user.UserId = g.Id;
                user.ConnectionId = Context.ConnectionId;

                OnlineGroup existingGroup = onlineGroups.FirstOrDefault(g => g.GroupId == sessiePartner.SessieId);
                if(existingGroup != null)
                {
                    OnlineUser sameUser = existingGroup.OnlineUsers.FirstOrDefault(u => u.UserId == user.UserId);
                    if(sameUser != null) {
                        //await Clients.Caller.JoinedFailed("This user has already joined (userId)"); 
                        await Clients.Caller.ServerMessage(1, "This user has already joined (userId)");
                        return; 
                    }

                    existingGroup.OnlineUsers.Add(user);
                    //await Clients.Caller.JoinedSuccess();
                    await Clients.Caller.ServerMessage(3, "You joined as a user");
                    await UpdateOnlineGroups();
                    if (existingGroup.Coordinator != null)
                    {
                        await Clients.Caller.CallUser(existingGroup.Coordinator);
                    }
                    foreach (OnlineUser partner in existingGroup.OnlineUsers)
                    {
                        if (partner.UserId != user.UserId)
                        {
                            await Clients.Caller.CallUser(partner);
                        }
                    }
                }
                else
                {
                    OnlineGroup newGroup = new OnlineGroup();
                    newGroup.GroupId = sessiePartner.SessieId;
                    newGroup.OnlineUsers.Add(user);
                    //await Clients.Caller.JoinedSuccess();
                    await Clients.Caller.ServerMessage(3, "You joined as a user");

                    onlineGroups.Add(newGroup);
                    await UpdateOnlineGroups();
                }
            }
            else
            {
                //await Clients.Caller.JoinedFailed("No session found which contains a user with id " + userId);
                await Clients.Caller.ServerMessage(4, "No session found which contains a user with id " + userId);
            }
        }
        //Check if user has coordinator role, if yes and group is online order him to call partners in group
        public async Task JoinGroupCallAsCoordinator(int groupId)
        {
            OnlineUser coordinator = onlineCoordinators.SingleOrDefault(c => c.ConnectionId == Context.ConnectionId);
            if(coordinator != null)
            {
                OnlineGroup existingGroup = onlineGroups.FirstOrDefault(g => g.GroupId == groupId);
                if (existingGroup != null)
                {
                    if(existingGroup.Coordinator != null) //only 1 coordinator allowed per group
                    {
                        return;
                    }

                    existingGroup.Coordinator = coordinator;
                    await UpdateOnlineGroups();
                    foreach (OnlineUser partner in existingGroup.OnlineUsers)
                    {
                        await Clients.Caller.CallUser(partner);
                    }
                }
                else
                {
                    await Clients.Caller.ServerMessage(6, "The group you are trying to join does not exist, groupId:" + groupId);
                }
            }
            else
            {
                await Clients.Caller.ServerMessage(5, "You are not logged in as a coordinator");
            }
        }
        //Let online partners in group know that user has left (if online partners in group = 0, remove online group)
        public async Task Leave()
        {
            OnlineUser coordinator = onlineCoordinators.SingleOrDefault(c => c.ConnectionId == Context.ConnectionId);
            if(coordinator != null) //user is a coordinator
            {
                OnlineGroup existingCorGroup = onlineGroups.FirstOrDefault(g => g.Coordinator == coordinator);
                if (existingCorGroup != null)
                {
                    existingCorGroup.Coordinator = null;
                    await UpdateOnlineGroups();

                    foreach (OnlineUser partner in existingCorGroup.OnlineUsers)
                    {
                        await Clients.Client(partner.ConnectionId).UserHasLeft(coordinator);
                    }
                }
                return;
            }

            OnlineGroup existingGroup = onlineGroups.FirstOrDefault(g => g.OnlineUsers.FirstOrDefault(u => u.ConnectionId == Context.ConnectionId) != null);
            if(existingGroup != null)
            {
                OnlineUser user = existingGroup.OnlineUsers.FirstOrDefault(u => u.ConnectionId == Context.ConnectionId);
                if(user != null)
                {
                    if(existingGroup.OnlineUsers.Count <= 1) //remove group, cuz last person is about to leave
                    {
                        if(existingGroup.Coordinator != null)
                        {
                            await Clients.Client(existingGroup.Coordinator.ConnectionId).UserHasLeft(user);
                        }
                        onlineGroups.Remove(existingGroup);
                        await UpdateOnlineGroups();
                    }
                    else
                    {
                        existingGroup.OnlineUsers.Remove(user);
                        foreach (OnlineUser u in existingGroup.OnlineUsers)
                        {
                            await Clients.Client(u.ConnectionId).UserHasLeft(user);
                        }
                        if (existingGroup.Coordinator != null)
                        {
                            await Clients.Client(existingGroup.Coordinator.ConnectionId).UserHasLeft(user);
                        }
                        await UpdateOnlineGroups();
                    }
                }
            }
        }
        //Send individual message from one user to another
        public async Task SendSignal(int toUserId, string data)
        {
            OnlineGroup existingGroup = onlineGroups.FirstOrDefault(g => g.OnlineUsers.FirstOrDefault(u => u.ConnectionId == Context.ConnectionId) != null || g.Coordinator?.ConnectionId == Context.ConnectionId);
            if (existingGroup != null)
            {
                OnlineUser sender = existingGroup.OnlineUsers.FirstOrDefault(u => u.ConnectionId == Context.ConnectionId);
                if(sender == null) { sender = existingGroup.Coordinator; }
                OnlineUser receiver = existingGroup.OnlineUsers.FirstOrDefault(u => u.UserId == toUserId);
                if(receiver == null) { receiver = existingGroup.Coordinator.UserId == toUserId ? existingGroup.Coordinator : null; }
                if (sender != null && receiver != null)
                {
                    await Clients.Client(receiver.ConnectionId).ReceiveSignal(sender, data);
                }
                else
                {
                    await Clients.Caller.ServerMessage(8, "You are not in the same group (atm) with the receiving user, thus you cannot signal this user with id:" + toUserId);
                }
            }
            else
            {
                await Clients.Caller.ServerMessage(7, "You are not part of any group and thus cannot signal anyone");
            }
        }
        //Send all online groups to coordinators (in call overview)
        public async Task UpdateOnlineGroups()
        {
            foreach (OnlineUser u in onlineCoordinators)
            {
                await Clients.Client(u.ConnectionId).ReceiveOnlineGroups(onlineGroups);
            }
        }
    }
}
