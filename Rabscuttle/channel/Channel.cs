using System.Collections.Generic;
using System.Linq;
using Rabscuttle.networking.commands;

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
                user += ((int)userRelation.Permission).ToString("x") + userRelation.user.userName + " ";
            }
            return "CHANNEL> N: [ " + channelName + " ] | U: [ " + user + "]";
        }

        protected bool Equals(Channel other) {
            return string.Equals(channelName, other.channelName);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Channel) obj);
        }

        public override int GetHashCode() {
            unchecked {
                return ((channelName != null ? channelName.GetHashCode() : 0) * 397) ^ (users != null ? users.GetHashCode() : 0);
            }
        }

        private sealed class ChannelNameUsersEqualityComparer : IEqualityComparer<Channel> {
            public bool Equals(Channel x, Channel y) {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return string.Equals(x.channelName, y.channelName) && Equals(x.users, y.users);
            }

            public int GetHashCode(Channel obj) {
                unchecked {
                    return ((obj.channelName != null ? obj.channelName.GetHashCode() : 0) * 397) ^ (obj.users != null ? obj.users.GetHashCode() : 0);
                }
            }
        }

        public static IEqualityComparer<Channel> ChannelNameUsersComparer { get; } = new ChannelNameUsersEqualityComparer();
    }
}
