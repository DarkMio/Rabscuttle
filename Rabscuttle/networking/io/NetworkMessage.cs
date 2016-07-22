using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Rabscuttle.networking.commands;

namespace Rabscuttle.networking {
    public class NetworkMessage {

        public readonly string message;
        public readonly string prefix;
        public readonly string type;
        public readonly string typeParams;
        public readonly bool fromServer;
        public readonly Enum typeEnum;

        public bool fromClient => !fromServer;

        private readonly Regex r = new Regex(
            @"^(?::(?<prefix>(?<user>[^@!\ ]*)(?:(?:\!(?<ident>[^@]*))?@(?<host>[^\ ]*))?)\ )?"+
            @"(?<type>[^\ ]+)(?<typeparameter>(?:\ [^:\ ][^\ ]*){0,14})(?:\ :?(?<message>.*))?(?:\r\n)?$",
            RegexOptions.Compiled
        );

        public NetworkMessage(string prefix, string typeParams, string message, bool fromServer, Enum type) {
            this.message = message;
            this.prefix = prefix;
            this.type = type + "";
            this.typeParams = typeParams;
            this.fromServer = fromServer;
            this.typeEnum = type;
        }

        public NetworkMessage(string prefix, string type, string typeParams, string message, bool fromServer) {
            this.message = message;
            this.prefix = prefix;
            this.type = type;
            this.typeParams = typeParams;
            this.fromServer = fromServer;
        }

        public NetworkMessage(string raw, bool fromServer) {
            try {
                var messageContent = r.Matches(raw)[0].Groups;
                prefix = messageContent["prefix"].Value;
                { // some IRC commands come as reply number, some as actual command.
                    var rawType = messageContent["type"].Value;
                    int replyCode;
                    bool isNumeric = int.TryParse(rawType, out replyCode);
                    type = isNumeric ? ((ReplyCode) replyCode).ToString() : rawType;
                }
                var typeParameter = messageContent["typeparameter"].Value;
                typeParams = String.IsNullOrWhiteSpace(typeParameter) ? typeParameter : typeParameter.Substring(1, typeParameter.Length - 1); // x.Value.Substring(1, x.Length-1);
                message = messageContent["message"].Value;

                if (Enum.IsDefined(typeof(ReplyCode), type)) {
                    typeEnum = (ReplyCode) Enum.Parse(typeof(ReplyCode), type, true);
                } else if (Enum.IsDefined(typeof(CommandCode), type)) {
                    typeEnum = (CommandCode) Enum.Parse(typeof(CommandCode), type, true);
                } else {
                    Debug.WriteLine("!! Unrecorginzed method: " + raw);
                }
                // message = messageContent[6].Value.Substring(1, messageContent[6].Value.Length).Replace("\r", "");
            } catch (ArgumentOutOfRangeException) {
                throw new ArgumentException("Misaligned Message: " + raw);
            }
            this.fromServer = fromServer;
        }

        public string BuildMessage() {
            string prefix = this.prefix != null ? ":" + this.prefix : "";
            string message = this.message != null ? " :" + this.message : "";
            return prefix + " " + type + " " + typeParams + message + "\r\n";
        }


        public override string ToString() {
            string origin = fromServer ? "SERVER> " : "CLIENT> ";
            return origin + "P: [ " + prefix + " ] | T:[ " + type + " ] | TP: [ " + typeParams + " ] | M: [ " + message + " ];";
        }
    }
}
