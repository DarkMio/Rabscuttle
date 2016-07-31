﻿using System.Collections.Generic;
using Rabscuttle.networking.commands;

namespace Rabscuttle.channel {
    public class UserRelation {
        public readonly ChannelUser user;
        public MemberCode permission;

        // @TODO: Permission parsing seems to be shit right now
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
