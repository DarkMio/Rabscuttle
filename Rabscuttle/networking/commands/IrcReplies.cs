using System;
using System.Text.RegularExpressions;
using Rabscuttle.networking.io;

namespace Rabscuttle.networking.commands {
    public abstract class IrcReply {
        public abstract ReplyCode Type { get; }
        public abstract bool HasTypeParameter { get; }
        public abstract bool HasMessage { get; }
    }

    public class WhoReply : IrcReply {
        public override ReplyCode Type => ReplyCode.RPL_WHOREPLY;
        public override bool HasTypeParameter => true;
        public override bool HasMessage => false;

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

        private static readonly Regex MODES_REGEX = new Regex(@"^([H|G])([*@%+.!]+)?.*$", RegexOptions.None);

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

            var matches = MODES_REGEX.Matches(parameter[6]);
            var options = matches[0].Groups;
            gone = String.Equals(options[1].Value, "h", StringComparison.CurrentCultureIgnoreCase);
            modes = options[2].Value;
            clientmodes = options[3].Value;

            Int32.TryParse(name[0], out hops);
            realname = name[1];
        }
    }
}
