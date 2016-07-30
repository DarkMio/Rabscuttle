﻿
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Rabscuttle.core.commands;
using Rabscuttle.core.io;

namespace Rabscuttle.core.channel {
    public class ChannelUser {
        public string userName;
        // public bool loggedIn;
        public string ident;
        public string name;
        public string host;
        public string realname;
        public string server;
        public int hops;

        public enum LoginStatus {Default, LoggedOut, LoggedIn }

        public LoginStatus loggedIn;
        public string loginUserName; // This is maybe short thought - maybe needs an instance of User


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

        public void ParseLoginMessage(NetworkMessage message) {
            if ((ReplyCode) message.typeEnum != ReplyCode.RPL_WHOISACCOUNT) {
                return;
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

        public void TryAddData(string source) {
            if (ident == null) {
                SetUserdata(source);
            }
        }

        public void TryAddData(string ident, string host, string server, int hops, string realname) {
            if (this.ident == null) {
                this.ident = ident;
            }
            if (this.host == null) {
                this.host = host;
            }
            if (this.server == null) {
                this.server = server;
            }
            if (this.hops == 0) {
                this.hops = hops;
            }
            if (this.realname == null) {
                this.realname = realname;
            }
        }

        public override string ToString() {
            return "USER  > UN: [ " + userName + " ] | I: [ " + ident + " ] | H: [ " + host + " ] | L: [ " + loggedIn + " ];";
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
