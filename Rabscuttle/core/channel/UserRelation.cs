using System.Collections.Generic;
using Rabscuttle.core.channel;
using Rabscuttle.networking.commands;

namespace Rabscuttle.core.channel {
    public class UserRelation {
        public readonly ChannelUser user;
        public MemberCode permission;

        public UserRelation(ChannelUser user, MemberCode permission) {
            this.user = user;
            this.permission = permission;
        }

        public void RemoveRank(MemberCode permission) {
            if (this.permission <= permission) {
                this.permission = MemberCode.DEFAULT;
            }
        }

        public void AddRank(MemberCode permission) {
            if (this.permission < permission) {
                this.permission = permission;
            }
        }

        protected bool Equals(UserRelation other) {
            return Equals(user, other.user) && permission == other.permission;
        }

        private sealed class UserPermissionEqualityComparer : IEqualityComparer<UserRelation> {
            public bool Equals(UserRelation x, UserRelation y) {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return Equals(x.user, y.user) && x.permission == y.permission;
            }

        public int GetHashCode(UserRelation obj) {
                unchecked {
                    return ((obj.user != null ? obj.user.GetHashCode() : 0) * 397) ^ (int) obj.permission;
                }
            }
        }

        private static readonly IEqualityComparer<UserRelation> UserPermissionComparerInstance = new UserPermissionEqualityComparer();

        public static IEqualityComparer<UserRelation> UserPermissionComparer {
            get { return UserPermissionComparerInstance; }
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((UserRelation) obj);
        }

        public override int GetHashCode() {
            unchecked {
                return ((user != null ? user.GetHashCode() : 0) * 397) ^ (int) permission;
            }
        }
    }
}
