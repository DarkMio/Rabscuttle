
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


        public string source {
            get {
                if (userName != null && ident != null && host != null) {
                    return userName + "!" + ident + "@" + host;
                }
                return null;
            }
        }

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
                SetUserdata(identOrUser);
            } else {
                userName = userRegex.Match(identOrUser).Value;
            }
        }

        public void SetUserdata(string dataString=null, string realname = null) {
            if (dataString != null) {
                if (ident == null) {
                    var results = identRegex.Matches(dataString)[0].Groups;
                    userName = results["user"].Value;
                    ident = results["ident"].Value;
                    host = results["host"].Value;
                } else {
                    throw new ArgumentException("Userdata is already set.");
                }
            }

            if (realname != null) {
                if (this.realname == null) {
                    this.realname = realname;
                } else {
                    throw new ArgumentException("Realname is already set");
                }
            }

        }

        public override string ToString() {
            return "USER  > UN: [ " + userName + " ] | I: [ " + ident + " ] | H: [ " + host + " ];";
        }

        protected bool Equals(ChannelUser other) {
            return string.Equals(userName, other.userName);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ChannelUser) obj);
        }

        public override int GetHashCode() {
            unchecked {
                return (userName != null ? userName.GetHashCode() : 0);
            }
        }

        private sealed class ChannelUserEqualityComparer : IEqualityComparer<ChannelUser> {
            public bool Equals(ChannelUser x, ChannelUser y) {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return string.Equals(x.userName, y.userName);
            }

            public int GetHashCode(ChannelUser obj) {
                unchecked {
                    return (obj.userName != null ? obj.userName.GetHashCode() : 0);
                }
            }
        }

        private static readonly IEqualityComparer<ChannelUser> ChannelUserComparerInstance = new ChannelUserEqualityComparer();

        public static IEqualityComparer<ChannelUser> ChannelUserComparer {
            get { return ChannelUserComparerInstance; }
        }
    }
}
