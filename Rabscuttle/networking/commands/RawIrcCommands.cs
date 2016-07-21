using System;
using System.Runtime.CompilerServices;

namespace Rabscuttle.networking.commands {
    public abstract class RawCommand<T> where T : RawCommand<T>, new() {

        public virtual string type { get; }
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

   
    

    public class Join : RawCommand<Join> {
        public override string type => "JOIN";
        public override bool hasTypeParameter => false;
        public override bool hasMessage => true;
    }

    public class Nick : RawCommand<Nick> {
        public override string type => "NICK";
        public override bool hasTypeParameter => true;
        public override bool hasMessage => false;
    }

    public class User : RawCommand<User> {
        public override string type => "USER";
        public override bool hasTypeParameter => true;
        public override bool hasMessage => true;
    }

    public class Ping : RawCommand<Ping> {
        public override string type => "PING";
        public override bool hasTypeParameter => true;
        public override bool hasMessage => false;
    }

    public class Pong : RawCommand<Pong> {
        public override string type => "PONG";
        public override bool hasTypeParameter => true;
        public override bool hasMessage => false;
    }

    public class PrivMsg : RawCommand<PrivMsg> {
        public override string type => "PRIVMSG";
        public override bool hasTypeParameter => true;
        public override bool hasMessage => true;
    }

    public class Part : RawCommand<Part> {
        public override string type => CommandCodes.PART + "";
        public override bool hasTypeParameter => true;
        public override bool hasMessage => true;
    }
}
