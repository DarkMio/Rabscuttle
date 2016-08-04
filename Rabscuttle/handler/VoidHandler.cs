using Rabscuttle.networking.io;

namespace Rabscuttle.handler {
    /// <summary> Sole purpose to dump all network messages in it - everyone is free to subscribe </summary>
    /// <seealso cref="Rabscuttle.handler.ObservableHandler" />
    public class VoidHandler : ObservableHandler {
        public override void HandleCommand(NetworkMessage message) {
            NotifyAll(message);
        }

        public override void HandleReply(NetworkMessage message) {
            NotifyAll(message);
        }
    }
}
