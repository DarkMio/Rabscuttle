
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Rabscuttle.core.channel {
    public class ChannelUser {
        public string userName;
        public bool loggedIn;
        public string ident;
        public string name;
        public string host;
        public string realname;

        private static Regex identRegex = new Regex(@"(?<user>[^@!\ ]*)(?:(?:\!(?<ident>[^@]*))?@(?<host>[^\ ]*))?", RegexOptions.Compiled);
        private static Regex userRegex = new Regex(@"([^+%@! ]+)", RegexOptions.Compiled);

        /*
        public ChannelUser(string userName, bool loggedIn) {
            this.userName = userName;
            this.loggedIn = loggedIn;
        }
        */
        public ChannelUser(string identOrUser, bool isUserName=false) {

            if (!isUserName) {
                var results = identRegex.Matches(identOrUser)[0].Groups;
                userName = results["user"].Value;
                ident = results["ident"].Value;
                host = results["host"].Value;
            } else {
                userName = userRegex.Match(identOrUser).Value;
            }

            Debug.WriteLine("GENERATED> " + this);
        }

        public void SetUserdata(string dataString=null, string realname = null) {
            if (dataString != null) {
                if (ident == null) {
                    ident = "uh oh";
                } else {
                    throw new ArgumentException("Userdata is already set.");
                }
            }

            if (realname != null) {
                if (this.realname == null) {
                    realname = "John Doe";
                } else {
                    throw new ArgumentException("Realname is already set");
                }
            }

        }

        public override string ToString() {
            return "USER  > UN: [ " + userName + " ] | I: [ " + ident + " ] | H: [ " + host + " ];";
        }

        protected bool Equals(ChannelUser other) {
            return string.Equals(userName, other.userName) && loggedIn == other.loggedIn && string.Equals(ident, other.ident) && string.Equals(name, other.name) && string.Equals(host, other.host) && string.Equals(realname, other.realname);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ChannelUser) obj);
        }

        public override int GetHashCode() {
            unchecked {
                var hashCode = (userName != null ? userName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ loggedIn.GetHashCode();
                hashCode = (hashCode * 397) ^ (ident != null ? ident.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (name != null ? name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (host != null ? host.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (realname != null ? realname.GetHashCode() : 0);
                return hashCode;
            }
        }

        private sealed class ChannelUserEqualityComparer : IEqualityComparer<ChannelUser> {
            public bool Equals(ChannelUser x, ChannelUser y) {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return string.Equals(x.userName, y.userName) && x.loggedIn == y.loggedIn && string.Equals(x.ident, y.ident) && string.Equals(x.name, y.name) && string.Equals(x.host, y.host) && string.Equals(x.realname, y.realname);
            }

            public int GetHashCode(ChannelUser obj) {
                unchecked {
                    var hashCode = (obj.userName != null ? obj.userName.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ obj.loggedIn.GetHashCode();
                    hashCode = (hashCode * 397) ^ (obj.ident != null ? obj.ident.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (obj.name != null ? obj.name.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (obj.host != null ? obj.host.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (obj.realname != null ? obj.realname.GetHashCode() : 0);
                    return hashCode;
                }
            }
        }

        private static readonly IEqualityComparer<ChannelUser> ChannelUserComparerInstance = new ChannelUserEqualityComparer();

        public static IEqualityComparer<ChannelUser> ChannelUserComparer {
            get { return ChannelUserComparerInstance; }
        }
    }
}
