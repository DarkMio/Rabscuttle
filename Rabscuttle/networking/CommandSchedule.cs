using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Rabscuttle.networking.io;

namespace Rabscuttle.networking {
    /// <summary>
    /// Handles message sending based on timing. Currently sends every 1.5s, but will get burst-sendings.
    /// </summary>
    public class CommandSchedule {
        private Stack<NetworkMessage> stack;
        private ISender client;
        private Task currentTask;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandSchedule"/> class and initiliazes a new instance of Stack.
        /// </summary>
        /// <param name="client">Any kind of sender, hopefully connected to any endpoint.</param>
        public CommandSchedule(ISender client){
            stack = new Stack<NetworkMessage>();
            this.client = client;
        }

        /// <summary>
        /// Adds a new network message and gets dispatched as soon as all previous messages are sent.
        /// </summary>
        /// <param name="item">Any <see cref="NetworkMessage"/> to be sent.</param>
        public void Add(NetworkMessage item) {
           stack.Push(item);
            if (currentTask == null || currentTask.IsCompleted) {
                currentTask = Send();
            }
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        /// <summary>
        /// Starts sending messages currently sitting on the stack.
        /// </summary>
        /// <returns>A receivable task, used to identify if the current sending-task is done or not.</returns>
        async Task Send() {
            while (stack.Count > 0) {
                client.Send(stack.Pop());
                Thread.Sleep(1500); // this needs a strategy at some point.
            }
        }
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    }
}
