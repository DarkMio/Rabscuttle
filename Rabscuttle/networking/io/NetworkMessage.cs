using System;
using System.Linq;
using Rabscuttle.networking.commands;
using Rabscuttle.util;

namespace Rabscuttle.networking.io {
    public class NetworkMessage {

        public readonly string message;
        public readonly string prefix;
        public readonly string type;
        public readonly string typeParams;
        public readonly bool fromServer;
        public readonly Enum typeEnum;

        public bool FromClient => !fromServer;

        /*
        private readonly Regex r = new Regex(
            @"^(?::(?<prefix>(?<user>[^@!\ ]*)(?:(?:\!(?<ident>[^@]*))?@(?<host>[^\ ]*))?)\ )?"+
            @"(?<Type>[^\ ]+)(?<typeparameter>(?:\ [^:\ ][^\ ]*){0,14})(?:\ :?(?<message>.*))?(?:\r\n)?$",
            RegexOptions.Compiled
        );

        private readonly Regex Source = new Regex(@":(?<user>[^@!\ ]*)\!(?<ident>[^@]*)@(?<host>[^\ ]*)", RegexOptions.Compiled);
        */

        public NetworkMessage(string prefix, string typeParams, string message, bool fromServer, Enum type) {
            this.message = message;
            this.prefix = prefix;
            this.type = type + "";
            this.typeParams = typeParams;
            this.fromServer = fromServer;
            typeEnum = type;
        }

        public NetworkMessage(string prefix, string type, string typeParams, string message, bool fromServer) {
            this.message = message;
            this.prefix = prefix;
            this.type = type;
            this.typeParams = typeParams;
            this.fromServer = fromServer;
        }

        /* Old constructor.
        public NetworkMessage(string raw, bool fromServer) {
            try {
                var messageContent = r.Matches(raw)[0].Groups;
                prefix = messageContent["prefix"].Value;
                { // some IRC commands come as reply number, some as actual command.
                    var rawType = messageContent["Type"].Value;
                    int replyCode;
                    bool isNumeric = int.TryParse(rawType, out replyCode);
                    Type = isNumeric ? ((ReplyCode) replyCode).ToString() : rawType;
                }
                var typeParameter = messageContent["typeparameter"].Value;
                typeParams = String.IsNullOrWhiteSpace(typeParameter) ? typeParameter : typeParameter.Substring(1, typeParameter.Length - 1); // x.Value.Substring(1, x.Length-1);
                message = messageContent["message"].Value;

                if (Enum.IsDefined(typeof(ReplyCode), Type)) {
                    typeEnum = (ReplyCode) Enum.Parse(typeof(ReplyCode), Type, true);
                } else if (Enum.IsDefined(typeof(CommandCode), Type)) {
                    typeEnum = (CommandCode) Enum.Parse(typeof(CommandCode), Type, true);
                } else {
                    Debug.WriteLine("!! Unrecorginzed method: " + raw);
                }
                // message = messageContent[6].Value.Substring(1, messageContent[6].Value.Length).Replace("\r", "");
            } catch (ArgumentOutOfRangeException) {
                throw new ArgumentException("Misaligned Message: " + raw);
            }
            this.fromServer = fromServer;
        }
        */

        public NetworkMessage(string raw, bool fromServer) {
            this.fromServer = fromServer;
            // Split the message for " :", since it would be only the message itself
            string[] messageSplit = raw.Split(new[] {" :"}, 2,  StringSplitOptions.None);
            // then, if there is anything, use the second part and join it (since the message could contain " :"
            message = messageSplit.Length > 1 ? string.Join(" :", messageSplit.Skip(1)) : null;

            // Now split up everything else.
            string[] commandStack = messageSplit[0].Split(' ');
            if (commandStack[0].StartsWith(":")) { // the Source string is the only one which starts with ":"
                prefix = commandStack[0].Substring(1);
                // if there was a Source string, then slice the array forwards.
                commandStack = commandStack.Skip(1).ToArray();
            }

            // following after is the Type, which is either plaintext or a reply-number.
            var rawType = commandStack[0];
            int replyCode;
            bool isNumeric = int.TryParse(rawType, out replyCode);
            // so we transform it into a string.
            type = isNumeric ? ((ReplyCode) replyCode).ToString() : rawType;
            // jump one over
            commandStack = commandStack.Skip(1).ToArray();

            // if now anything is left, it should be Type-parameter.
            typeParams = commandStack.Length > 0 ? string.Join(" ", commandStack) : null;

            // and finally write out the Type-params.
            if (Enum.IsDefined(typeof(ReplyCode), type)) {
                typeEnum = (ReplyCode) replyCode;
            } else if (Enum.IsDefined(typeof(CommandCode), type)) {
                typeEnum = (CommandCode) Enum.Parse(typeof(CommandCode), type, true);
            } else {
                Logger.WriteWarn("NetworkMessage", "Unrecorgnized method: [ {0} ] in: [ {1} ]", type, raw);
            }
        }

        public string BuildMessage() {
            string msgPrefix = prefix != null ? ":" + prefix : "";
            string msgMessage = message != null ? " :" + message : "";
            return msgPrefix + " " + type + " " + typeParams + msgMessage + "\r\n";
        }


        public override string ToString() {
            string origin = fromServer ? "SERVER> " : "CLIENT> ";
            return origin + "P: [ " + prefix + " ] | T: [ " + type + " ] | TP: [ " + typeParams + " ] | M: [ " + message + " ];";
        }

        protected bool Equals(NetworkMessage other) {
            return string.Equals(message, other.message) && string.Equals(prefix, other.prefix)
                && string.Equals(type, other.type) && string.Equals(typeParams, other.typeParams)
                && fromServer == other.fromServer && Equals(typeEnum, other.typeEnum);
        }

        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        /// <returns>true if the specified object  is equal to the current object; otherwise, false.</returns>
        /// <param name="obj">The object to compare with the current object. </param>
        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((NetworkMessage) obj);
        }

        /// <summary>Serves as the default hash function. </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode() {
            unchecked {
                int hashCode = message?.GetHashCode() ?? 0;
                hashCode = (hashCode * 397) ^ (prefix?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (type?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (typeParams?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ fromServer.GetHashCode();
                hashCode = (hashCode * 397) ^ (typeEnum?.GetHashCode() ?? 0);
                return hashCode;
            }
        }
    }
}
