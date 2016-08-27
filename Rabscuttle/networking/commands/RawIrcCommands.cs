using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web.Security;
using Rabscuttle.networking.io;

namespace Rabscuttle.networking.commands {
    public abstract class RawCommand<T> where T : RawCommand<T>, new() {
        public abstract CommandCode Type { get; }
        public abstract bool HasTypeParameter { get; }
        public abstract bool HasMessage { get; }
        public static T Instance { get; } = new T();
        private const int MAX_BYTES = 512;
        private static readonly int SINGLE_BYTE_SIZE = System.Text.Encoding.Unicode.GetByteCount(new [] {' '});

        protected NetworkMessage InstanceGenerate(bool fromServer, string prefix = null, string typeParameter = null,
                                                  string message = null) {
            if (Type == CommandCode.DEFAULT) {
                throw new ArgumentException("This method cannot be used by any implementation of CommandCode.DEFAULT!");
            }
            return InstanceRawGenerate(fromServer, Type, prefix, typeParameter, message);
        }

        protected NetworkMessage InstanceRawGenerate(bool fromServer, Enum type, string prefix = null,
                                                  string typeParameter = null, string message = null) {
            return new NetworkMessage(prefix, typeParameter, message, fromServer, type);
        }


        protected NetworkMessage InstanceRawGenerate(bool fromServer, string type, string prefix = null,
                                                  string typeParameter = null, string message = null) {
            return new NetworkMessage(prefix, type, typeParameter, message, fromServer);
        }

        protected NetworkMessage[] Generate(bool fromServer, string prefix = null, string typeParameter = null,
            string message = null) {
            if (HasTypeParameter && typeParameter == null) {
                throw new ArgumentException("For this Type [ " + Type + " ] typeParameter cannot be null!");
            }
            if (HasMessage && message == null) {
                throw new ArgumentException("For this Type [" + Type + " ] message cannot be null!");
            }

            if (message == null) {
                return new[] {Instance.InstanceGenerate(fromServer, prefix, typeParameter, message)};
            }

            // build a sample command without the message
            NetworkMessage byteMessage = Instance.InstanceGenerate(fromServer, prefix, typeParameter);
            // calculate its size and how much space a message can take.
            int byteSize = System.Text.Encoding.Unicode.GetByteCount(byteMessage.BuildMessage() + ":");
            int remainingBytes = MAX_BYTES - byteSize;
            int chunksPerLine = remainingBytes / SINGLE_BYTE_SIZE;

            // Split newlines
            string[] messageParts = message.Split(new[] {"\r\n", "\n"}, StringSplitOptions.RemoveEmptyEntries);
            List<NetworkMessage> messages = new List<NetworkMessage>();
            foreach (string messagePart in messageParts) {
                // get how much space this line will take
                int messageSize = System.Text.Encoding.Unicode.GetByteCount(messagePart);
                if (remainingBytes >= messageSize) { // if we have enough space, continue
                    messages.Add(Instance.InstanceGenerate(fromServer, prefix, typeParameter, messagePart));
                    continue;
                }
                // or else chunk the string and add all chunks to the commands
                IEnumerable<string> chunks = ChunksUpto(messagePart, chunksPerLine);
                foreach (string chunk in chunks) {
                    messages.Add(Instance.InstanceGenerate(fromServer, prefix, typeParameter, messagePart));
                }
            }
            return messages.ToArray();
        }

        static IEnumerable<string> ChunksUpto(string str, int maxChunkSize) {
            for (int i = 0; i < str.Length; i += maxChunkSize)
                yield return str.Substring(i, Math.Min(maxChunkSize, str.Length-i));
        }
    }

    public class RawJoin : RawCommand<RawJoin> {
        public override CommandCode Type => CommandCode.JOIN;
        public override bool HasTypeParameter => false;
        public override bool HasMessage => true;
        public static NetworkMessage Generate(string message, bool fromServer = false, string prefix = null) {
            return Instance.InstanceGenerate(fromServer, prefix, null, message);
        }
    }

    public class RawNick : RawCommand<RawNick> {
        public override CommandCode Type => CommandCode.NICK;
        public override bool HasTypeParameter => true;
        public override bool HasMessage => false;
        public static NetworkMessage Generate(string typeParameter, bool fromServer = false, string prefix = null) {
            return Instance.InstanceGenerate(fromServer, prefix, typeParameter);
        }
    }

    public class RawUser : RawCommand<RawUser> {
        public override CommandCode Type => CommandCode.USER;
        public override bool HasTypeParameter => true;
        public override bool HasMessage => true;
        public static NetworkMessage Generate(string typeParameter, string message, bool fromServer = false, string prefix = null) {
            return Instance.InstanceGenerate(fromServer, prefix, typeParameter, message);
        }
    }

    public class RawPing : RawCommand<RawPing> {
        public override CommandCode Type => CommandCode.PING;
        public override bool HasTypeParameter => true;
        public override bool HasMessage => false;
        public static NetworkMessage Generate(string typeParameter, bool fromServer = false, string prefix = null) {
            return Instance.InstanceGenerate(fromServer, prefix, typeParameter);
        }
    }

    public class RawPong : RawCommand<RawPong> {
        public override CommandCode Type => CommandCode.PONG;
        public override bool HasTypeParameter => true;
        public override bool HasMessage => false;
        public static NetworkMessage Generate(string typeParameter, bool fromServer = false, string prefix = null) {
            return Instance.InstanceGenerate(fromServer, prefix, typeParameter);
        }
    }

    public class RawPrivMsg : RawCommand<RawPrivMsg> {
        public override CommandCode Type => CommandCode.PRIVMSG;
        public override bool HasTypeParameter => true;
        public override bool HasMessage => true;
        public static NetworkMessage[] Generate(string typeParameter, string message, bool fromServer = false, string prefix = null) {
            return Instance.Generate(fromServer, prefix, typeParameter, message);
        }
    }

    public class RawPart : RawCommand<RawPart> {
        public override CommandCode Type => CommandCode.PART;
        public override bool HasTypeParameter => true;
        public override bool HasMessage => true;
        public static NetworkMessage Generate(string typeParameter, string message, bool fromServer = false, string prefix = null) {
            return Instance.InstanceGenerate(fromServer, prefix, typeParameter, message);
        }
    }

    public class RawMode : RawCommand<RawMode> {
        public override CommandCode Type => CommandCode.MODE;
        public override bool HasTypeParameter => true;
        public override bool HasMessage => true;
        public static NetworkMessage Generate(string typeParameter, string message, bool fromServer = false, string prefix = null) {
            return Instance.InstanceGenerate(fromServer, prefix, typeParameter, message);
        }
    }

    public class RawWho : RawCommand<RawWho> {
        public override CommandCode Type => CommandCode.WHO;
        public override bool HasTypeParameter => true;
        public override bool HasMessage => false;
        public static NetworkMessage Generate(string typeParameter, bool fromServer = false, string prefix = null) {
            return Instance.InstanceGenerate(fromServer, prefix, typeParameter);
        }
    }

    public class AuthServ : RawCommand<AuthServ> {
        public override CommandCode Type => CommandCode.DEFAULT;
        public override bool HasTypeParameter => true;
        public override bool HasMessage => false;

        public static NetworkMessage Generate(string typeParameter, bool fromServer = false, string prefix = null) {
            return Instance.InstanceRawGenerate(fromServer, "AUTHSERV", prefix, typeParameter);
        }
    }

    public class RawWhois : RawCommand<RawWhois> {
        public override CommandCode Type => CommandCode.WHOIS;
        public override bool HasTypeParameter => false;
        public override bool HasMessage => false;

        public static NetworkMessage Generate(string message, bool fromServer = false, string prefix = null) {
            return Instance.InstanceGenerate(fromServer, prefix, null, message);
        }
    }

    public class RawNotice : RawCommand<RawNotice> {
        public override CommandCode Type => CommandCode.NOTICE;
        public override bool HasTypeParameter => true;
        public override bool HasMessage => true;

        public static NetworkMessage[] Generate(string typeParameter, string message, bool fromServer = false,
            string prefix = null) {

            return Instance.Generate(fromServer, prefix, typeParameter, message);
            // return Instance.InstanceGenerate(fromServer, prefix, typeParameter, message);
        }
    }

    public class RawQuit : RawCommand<RawQuit> {
        public override CommandCode Type => CommandCode.QUIT;
        public override bool HasTypeParameter => false;
        public override bool HasMessage => false;

        public static NetworkMessage Generate(bool fromServer = false, string prefix = null) {
            return Instance.InstanceGenerate(fromServer, prefix);
        }
    }
}
