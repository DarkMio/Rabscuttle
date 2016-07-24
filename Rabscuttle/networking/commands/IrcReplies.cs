using System;
using System.Text.RegularExpressions;
using Rabscuttle.networking.io;

namespace Rabscuttle.networking.commands {
    public abstract class IrcReply {
        public abstract ReplyCode type { get; }
        public abstract bool hasTypeParameter { get; }
        public abstract bool hasMessage { get; }
    }

    public class WhoReply : IrcReply {
        public override ReplyCode type => ReplyCode.RPL_WHOREPLY;
        public override bool hasTypeParameter => true;
        public override bool hasMessage => false;

        public string channel;
        public string ident;
        public string host;
        public string server;
        public string username;
        public bool gone;
        public string modes;
        public string clientmodes;
        public int hops;
        public string realname;

        private static Regex s = new Regex(@"^([H|G])([*@%+.!]+)?.*$", RegexOptions.None);

        public WhoReply(NetworkMessage message) {
            var parameter = message.typeParams.Split(new char[] {' '});
            var name = message.message.Split(new char[] {' '});
            if (parameter.Length < 7 || name.Length < 2) {
                throw new ArgumentException("Misaligned Message? Was: " + message);
            }

            channel = parameter[1];
            ident = parameter[2];
            host = parameter[3];
            server = parameter[4];
            username = parameter[5];

            var matches = s.Matches(parameter[6]);
            var options = matches[0].Groups;
            gone = String.Equals(options[1].Value, "h", StringComparison.CurrentCultureIgnoreCase);
            modes = options[2].Value;
            clientmodes = options[3].Value;

            Int32.TryParse(name[0], out hops);
            realname = name[1];
        }
    }
}
