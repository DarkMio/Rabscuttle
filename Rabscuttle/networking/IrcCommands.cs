using System;

namespace Rabscuttle.networking.commands {
    public abstract class Command<T> where T : Command<T>, new() {

        public abstract string type { get; }
        public abstract bool hasTypeParameter { get; }
        public abstract bool hasMessage { get; }

        public static T Instance { get; } = new T();

        public NetworkMessage Generate(bool fromServer, string prefix=null, string typeParameter = null, string message = null) {
            if (type == null) {
                throw new ArgumentException("Type cannot be null.");
            }
            if (hasTypeParameter && typeParameter == null) {
                throw new ArgumentException("For this type [ " + type + " ] typeParameter cannot be null!");
            }
            if (hasMessage && message == null) {
                throw new ArgumentException("For this type [" + type + " ] message cannot be null!");
            }

            return new NetworkMessage(prefix, type, typeParameter, message, fromServer);
        }
    }

    public class Join : Command<Join> {
        public override string type => "JOIN";
        public override bool hasTypeParameter => false;
        public override bool hasMessage => true;
    }

    public class Nick : Command<Nick> {
        public override string type => "NICK";
        public override bool hasTypeParameter => true;
        public override bool hasMessage => false;
    }

    public class User : Command<User> {
        public override string type => "USER";
        public override bool hasTypeParameter => true;
        public override bool hasMessage => true;
    }

    public class Ping : Command<Ping> {
        public override string type => "PING";
        public override bool hasTypeParameter => true;
        public override bool hasMessage => false;
    }

    public class Pong : Command<Pong> {
        public override string type => "PONG";
        public override bool hasTypeParameter => true;
        public override bool hasMessage => false;
    }

    public class PrivMsg : Command<PrivMsg> {
        public override string type => "PRIVMSG";
        public override bool hasTypeParameter => true;
        public override bool hasMessage => true;
    }
}
