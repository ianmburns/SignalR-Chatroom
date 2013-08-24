using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using SignalRChatRoom.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRChatRoom.Hubs
{
    [HubName("ChatHub")]
    public class ChatHub : Hub
    {
        private static readonly List<UserDetail> ConnectedUsers = new List<UserDetail>();

        private static readonly string[] ChatGroups = 
        {    
            "Lobby",
            "Games",
            "Sports",
            "Politics"
        };

        #region HelperMethods

        private UserDetail GetCurrentUser()
        {
            var user = ConnectedUsers.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            return user;
        }

        private List<UserDetail> GetUsersByGroup(string groupName)
        {
            return ConnectedUsers.Where(x => x.CurrentGroup == groupName).ToList();
        }

        #endregion

        #region Methods

        public void Connect(string userName)
        {
            if (ConnectedUsers.Any(x => x.ConnectionId == Context.ConnectionId))
                return;

            //Add a new user to the list
            var user = new UserDetail
            {
                ConnectionId = Context.ConnectionId,
                Name = userName
            };

            //Add to maintained user list
            ConnectedUsers.Add(user);

            //Add to default group
            //await 
            Groups.Add(user.ConnectionId, user.CurrentGroup);

            //Give the current group an updated list of users
            UpdateGroupUserList(user.CurrentGroup);

            //Alert others that user joined
            var message = string.Format("{0} has joined.", user.Name);
            SendGroupAlert(user, message);

            //Alert user that they have joined
            var personalMessage = string.Format("You have joined {0} as {1}.", user.CurrentGroup, user.Name);
            SendPersonalAlert(personalMessage);
        }

        public void JoinGroup(string groupName)
        {
            var user = GetCurrentUser();

            if (!ChatGroups.Contains(groupName))
            {
                SendPersonalAlert("No group exists by that name");
                return;
            }
            if (user.CurrentGroup.Equals(groupName))
            {
                SendPersonalAlert("You are already a member of that group");
                return;
            }

            //Remove from current group
            var oldGroup = user.CurrentGroup;

            //Tell everyone user is leaving
            var message = string.Format("{0} has left.", user.Name);
            SendGroupAlert(user, message);

            //remove user from the group
            //await 
            Groups.Remove(user.ConnectionId, oldGroup);

            //Join new Group
            user.CurrentGroup = groupName;
            //await 
            Groups.Add(user.ConnectionId, user.CurrentGroup);

            //Update the user lists for the old and new group
            UpdateGroupUserList(oldGroup);
            UpdateGroupUserList(user.CurrentGroup);

            message = string.Format("{0} has joined.", user.Name);
            SendGroupAlert(user, message);

            message = string.Format("You have joined {0}", user.CurrentGroup);
            SendPersonalAlert(message);
        }

        public override Task OnDisconnected()
        {
            var user = GetCurrentUser();

            if (user != null)
            {
                ConnectedUsers.Remove(user);
                UpdateGroupUserList(user.CurrentGroup);

                //Alert that user left
                var message = string.Format("{0} has left.", user.Name);
                SendGroupAlert(user, message);
            }

            return base.OnDisconnected();
        }
        #endregion

        #region Messages

        public void SendMessage(string message)
        {
            var user = GetCurrentUser();
            SendGroupMessage(user, message);
        }

        private void SendGroupMessage(UserDetail user, string message)
        {
            Clients.Group(user.CurrentGroup).addMessage(user.Name, message);
        }

        private void SendPersonalAlert(string message)
        {
            Clients.Caller.systemAlert(message);
        }

        /// <summary>
        /// Will send an alert to all members of the user's current group, except for that user
        /// </summary>
        private void SendGroupAlert(UserDetail user, string message)
        {
            Clients.Group(user.CurrentGroup, user.ConnectionId).systemAlert(message);
        }

        private void UpdateGroupUserList(string group)
        {
            var users = GetUsersByGroup(group);
            Clients.Group(group).refreshUserList(users);
        }

        #endregion
    }
}