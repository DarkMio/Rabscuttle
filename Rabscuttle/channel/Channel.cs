using System;
using System.Collections.Generic;
using System.Linq;
using Rabscuttle.core.channel;
using Rabscuttle.core;
using Rabscuttle.core.commands;

namespace Rabscuttle.channel {
    public class Channel{
        public string channelName;
        public HashSet<UserRelation> users;

        public Channel(string channelName) {
            this.channelName = channelName;
            users = new HashSet<UserRelation>();
        }

        public void AddUser(ChannelUser user, MemberCode permission) {
            users.Add(new UserRelation(user, permission));
        }

        public void RemoveUser(ChannelUser user) {
            users.Remove(users.SingleOrDefault(s => s.user == user));
        }

        public void RemoveRank(ChannelUser user, MemberCode permission) {
            UserRelation relation = users.Single(s => s.user == user);
            relation.RemoveRank(permission);
        }

        public void AddRank(ChannelUser user, MemberCode permission) {
            UserRelation relation = users.Single(s => s.user == user);
            relation.AddRank(permission);
        }

        public override string ToString() {
            string user = "";
            foreach (UserRelation userRelation in users) {
                user += ((int)userRelation.permission).ToString("x") + userRelation.user.userName + " ";
            }
            return "CHANNEL> N: [ " + channelName + " ] | U: [ " + user + "]";
        }
    }
}
