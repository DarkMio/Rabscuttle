using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Rabscuttle.networking {
    public class NetworkMessage {

        public readonly string message;
        public readonly string prefix;
        public readonly string type;
        public readonly string destination;
        public readonly bool fromServer;
        public bool fromClient => !fromServer;

        private readonly Regex r = new Regex(@"^(?:[:](\S+) )?(\S+)(?: (?!:)(.+?))?(?: [:](.+))[\r|\n]*$", RegexOptions.Compiled);

        public NetworkMessage(string message, string prefix, string type, string destination, bool fromServer) {
            this.message = message;
            this.prefix = prefix;
            this.type = type;
            this.destination = destination;
            this.fromServer = fromServer;
        }

        public NetworkMessage(string raw, bool fromServer) {
            try {
                var messageContent = r.Matches(raw)[0].Groups;
                prefix = messageContent[1].Value;
                type = messageContent[2].Value;
                destination = messageContent[3].Value;
                message = messageContent[4].Value.Replace("\r", "");
            } catch (ArgumentOutOfRangeException e) {
                throw new ArgumentException("Misaligned Message: " + raw);
            }
            this.fromServer = fromServer;
        }

        public string BuildMessage() {
            string prefix = this.prefix != null ? ":" + this.prefix : "";
            string message = this.message != null ? " :" + this.message : "";
            return prefix + " " + type + " " + destination + message + "\r\n";
    }


        public override string ToString() {
            string origin = fromServer ? "SERVER> " : "CLIENT> ";
            return origin + "P: [ " + prefix + " ] | T:[ " + type + " ] | D: [ " + destination + " ] | M: [ " + message + " ];";
        }
    }
}
