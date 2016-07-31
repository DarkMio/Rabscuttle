using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Rabscuttle.networking.io;

namespace Rabscuttle.networking {
    /// <summary>
    /// Handles message sending based on timing. Currently sends every 1.5s, but will get burst-sendings.
    /// </summary>
    public class CommandSchedule {
        private readonly Queue<NetworkMessage> _queue;
        private readonly ISender _client;
        private Task _currentTask;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandSchedule"/> class and initiliazes a new instance of Stack.
        /// </summary>
        /// <param name="client">Any kind of sender, hopefully connected to any endpoint.</param>
        public CommandSchedule(ISender client){
            _queue = new Queue<NetworkMessage>();
            _client = client;
        }

        /// <summary>
        /// Adds a new network message and gets dispatched as soon as all previous messages are sent.
        /// </summary>
        /// <param name="item">Any <see cref="NetworkMessage"/> to be sent.</param>
        public void Add(NetworkMessage item) {
            _queue.Enqueue(item);
            if (_currentTask == null || _currentTask.IsCompleted) {
                _currentTask = Send();
            }
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        /// <summary>
        /// Starts sending messages currently sitting on the queue.
        /// </summary>
        /// <returns>A receivable task, used to identify if the current sending-task is done or not.</returns>
        async Task Send() {
            while (_queue.Count > 0) {
                _client.Send(_queue.Dequeue());
                Thread.Sleep(1500); // this needs a strategy at some point.
            }
        }
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    }
}
