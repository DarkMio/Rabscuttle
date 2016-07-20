using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Rabscuttle.networking {
    public class CommandSchedule {
        private Stack<NetworkMessage> stack;
        private BotClient client;
        private Task currentTask;

        public CommandSchedule(BotClient client){
            stack = new Stack<NetworkMessage>();
            this.client = client;
        }

        public void Add(NetworkMessage item) {
           stack.Push(item);
            if (currentTask == null || currentTask.IsCompleted) {
                currentTask = Send();
            }
        }

        async Task Send() {
            while (stack.Count > 0) {
                client.Send(stack.Pop());
                Thread.Sleep(1500); // this needs a strategy at some point.
            }
        }
    }
}
