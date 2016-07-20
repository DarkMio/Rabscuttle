namespace Rabscuttle.networking.io {
    public interface ISender {
        void Send(string message, string prefix, string command, string commandParams);
        void Send(NetworkMessage message);
    }

    public interface IReceiver {
        NetworkMessage Receive(bool waitResponse=false);
    }
}
