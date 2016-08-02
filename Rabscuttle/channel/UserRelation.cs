using System;
using System.Collections.Generic;
using System.Globalization;
using Rabscuttle.networking.commands;

namespace Rabscuttle.channel {
    public class UserRelation {
        public readonly ChannelUser user;
        private int _permissions;

        public MemberCode Permission {
            get {
                if (_permissions == 0) {
                    return MemberCode.DEFAULT;
                }
                // following doesn't work when 0
                // get me the next upper of the exponent of 2
                var numBits = (int)Math.Ceiling(Math.Log(_permissions + 1, 2));
                // then shift it one time less (which is why it doesn't work with 0: 1 << -1 -> underflow)
                return (MemberCode) (1 << (numBits - 1));
            }
        }

        public UserRelation(ChannelUser user, MemberCode permission) {
            this.user = user;
            _permissions = 0;
            AddRank(permission);
        }

        public void RemoveRank(MemberCode permission) {
            _permissions &= ~(int) permission;
        }

        public void AddRank(MemberCode permission) {
            _permissions |= (int)permission;
        }

        protected bool Equals(UserRelation other) {
            return Equals(user, other.user);
        }

        private sealed class UserPermissionEqualityComparer : IEqualityComparer<UserRelation> {
            public bool Equals(UserRelation x, UserRelation y) {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return Equals(x.user, y.user);
            }

        public int GetHashCode(UserRelation obj) {
                unchecked {
                    return (obj.user?.GetHashCode() ?? 0) * 397;
                }
            }
        }

        public static IEqualityComparer<UserRelation> UserPermissionComparer { get; } = new UserPermissionEqualityComparer();

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((UserRelation) obj);
        }

        public override int GetHashCode() {
            unchecked {
                return (user?.GetHashCode() ?? 0) * 397;
            }
        }
    }
}
