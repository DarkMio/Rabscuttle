using System;

namespace Rabscuttle.exception {
    public class ConnectionClosedException : Exception {
        public bool FromServer { get; }

        public ConnectionClosedException(string message, bool fromServer) : base(message) {
            FromServer = fromServer;
        }

        public ConnectionClosedException(string message, Exception inner, bool fromServer) : base(message, inner) {
            FromServer = fromServer;
        }
    }
}
