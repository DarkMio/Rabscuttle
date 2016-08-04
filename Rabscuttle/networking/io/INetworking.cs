namespace Rabscuttle.networking.io {
    /// <summary>
    /// Sender Interface for sending any kind of IRC-related network messages in plaintext to any kind of endpoint.
    /// </summary>
    public interface ISender {
        /// <summary>
        /// Sends a properly aligned plaintext message to the endpoint, alignment looks like:
        /// <c>:Source* Type typeParam* :message*</c>
        /// </summary>
        /// <param name="message">Message contents, which are usually at the end of a string.</param>
        /// <param name="prefix">The Source prefix, client to server messages can have this missing.</param>
        /// <param name="type">Message Type, usually a <c>CommandCode</c> when sending from client to server.</param>
        /// <param name="typeParams">Optional, additional parameter for a message Type.</param>
        void Send(string message, string prefix, string type, string typeParams);
        /// <summary>
        /// Sends a <see cref="NetworkMessage"/> to the endpoint..
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        void Send(NetworkMessage message);
    }

    /// <summary>
    /// Receiver Interface for receiving any kind of IRC-related network messages in plaintext.
    /// </summary>
    public interface IReceiver {
        /// <summary>
        /// Receives the specified wait response from the endpoint.
        /// </summary>
        /// <param name="waitResponse">if set to <c>true</c> method shall wait until there is at least a single line of data.</param>
        /// <returns></returns>
        NetworkMessage Receive(bool waitResponse=false);
    }
}
