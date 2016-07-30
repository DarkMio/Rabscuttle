using System;
using Rabscuttle.core.io;

namespace Rabscuttle.core.commands {
    public abstract class RawCommand<T> where T : RawCommand<T>, new() {
        public abstract CommandCode type { get; }
        public abstract bool hasTypeParameter { get; }
        public abstract bool hasMessage { get; }
        public static T Instance { get; } = new T();

        protected NetworkMessage InstanceGenerate(bool fromServer, string prefix = null, string typeParameter = null,
                                                  string message = null) {
            if (type == CommandCode.DEFAULT) {
                throw new ArgumentException("This method cannot be used by any implementation of CommandCode.DEFAULT!");
            }
            return InstanceRawGenerate(fromServer, type, prefix, typeParameter, message);
        }

        protected NetworkMessage InstanceRawGenerate(bool fromServer, Enum type, string prefix = null,
                                                  string typeParameter = null, string message = null) {
            return new NetworkMessage(prefix, typeParameter, message, fromServer, type);
        }


        protected NetworkMessage InstanceRawGenerate(bool fromServer, string type, string prefix = null,
                                                  string typeParameter = null, string message = null) {
            return new NetworkMessage(prefix, type, typeParameter, message, fromServer);
        }
        protected NetworkMessage Generate(bool fromServer, string prefix = null, string typeParameter = null,
            string message = null) {
            if (hasTypeParameter && typeParameter == null) {
                throw new ArgumentException("For this type [ " + type + " ] typeParameter cannot be null!");
            }
            if (hasMessage && message == null) {
                throw new ArgumentException("For this type [" + type + " ] message cannot be null!");
            }

            return Instance.InstanceGenerate(fromServer, prefix, typeParameter, message);
        }
    }

    public class RawJoin : RawCommand<RawJoin> {
        public override CommandCode type => CommandCode.JOIN;
        public override bool hasTypeParameter => false;
        public override bool hasMessage => true;
        public static NetworkMessage Generate(string message, bool fromServer = false, string prefix = null) {
            return Instance.InstanceGenerate(fromServer, prefix, null, message);
        }
    }

    public class RawNick : RawCommand<RawNick> {
        public override CommandCode type => CommandCode.NICK;
        public override bool hasTypeParameter => true;
        public override bool hasMessage => false;
        public static NetworkMessage Generate(string typeParameter, bool fromServer = false, string prefix = null) {
            return Instance.InstanceGenerate(fromServer, prefix, typeParameter);
        }
    }

    public class RawUser : RawCommand<RawUser> {
        public override CommandCode type => CommandCode.USER;
        public override bool hasTypeParameter => true;
        public override bool hasMessage => true;
        public static NetworkMessage Generate(string typeParameter, string message, bool fromServer = false, string prefix = null) {
            return Instance.InstanceGenerate(fromServer, prefix, typeParameter, message);
        }
    }

    public class RawPing : RawCommand<RawPing> {
        public override CommandCode type => CommandCode.PING;
        public override bool hasTypeParameter => true;
        public override bool hasMessage => false;
        public static NetworkMessage Generate(string typeParameter, bool fromServer = false, string prefix = null) {
            return Instance.InstanceGenerate(fromServer, prefix, typeParameter);
        }
    }

    public class RawPong : RawCommand<RawPong> {
        public override CommandCode type => CommandCode.PONG;
        public override bool hasTypeParameter => true;
        public override bool hasMessage => false;
        public static NetworkMessage Generate(string typeParameter, bool fromServer = false, string prefix = null) {
            return Instance.InstanceGenerate(fromServer, prefix, typeParameter);
        }
    }

    public class RawPrivMsg : RawCommand<RawPrivMsg> {
        public override CommandCode type => CommandCode.PRIVMSG;
        public override bool hasTypeParameter => true;
        public override bool hasMessage => true;
        public static NetworkMessage Generate(string typeParameter, string message, bool fromServer = false, string prefix = null) {
            return Instance.InstanceGenerate(fromServer, prefix, typeParameter, message);
        }
    }

    public class RawPart : RawCommand<RawPart> {
        public override CommandCode type => CommandCode.PART;
        public override bool hasTypeParameter => true;
        public override bool hasMessage => true;
        public static NetworkMessage Generate(string typeParameter, string message, bool fromServer = false, string prefix = null) {
            return Instance.InstanceGenerate(fromServer, prefix, typeParameter, message);
        }
    }

    public class RawMode : RawCommand<RawMode> {
        public override CommandCode type => CommandCode.MODE;
        public override bool hasTypeParameter => true;
        public override bool hasMessage => true;
        public static NetworkMessage Generate(string typeParameter, string message, bool fromServer = false, string prefix = null) {
            return Instance.InstanceGenerate(fromServer, prefix, typeParameter, message);
        }
    }

    public class RawWho : RawCommand<RawWho> {
        public override CommandCode type => CommandCode.WHO;
        public override bool hasTypeParameter => true;
        public override bool hasMessage => false;
        public static NetworkMessage Generate(string typeParameter, bool fromServer = false, string prefix = null) {
            return Instance.InstanceGenerate(fromServer, prefix, typeParameter);
        }
    }

    public class AuthServ : RawCommand<AuthServ> {
        public override CommandCode type => CommandCode.DEFAULT;
        public override bool hasTypeParameter => true;
        public override bool hasMessage => false;

        public static NetworkMessage Generate(string typeParameter, bool fromServer = false, string prefix = null) {
            return Instance.InstanceRawGenerate(fromServer, "AUTHSERV", prefix, typeParameter);
        }
    }

    public class RawWhois : RawCommand<RawWhois> {
        public override CommandCode type => CommandCode.WHOIS;
        public override bool hasTypeParameter => false;
        public override bool hasMessage => false;

        public static NetworkMessage Generate(string message, bool fromServer = false, string prefix = null) {
            return Instance.InstanceGenerate(fromServer, prefix, null, message);
        }
    }
}
