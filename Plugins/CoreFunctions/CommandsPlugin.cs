using System.Linq;
using Rabscuttle.handler;
using Rabscuttle.networking.commands;
using Rabscuttle.networking.io;
using Rabscuttle.plugin;

namespace CoreFunctions {
    public class CommandsPlugin : IPluginContract {
        public string CommandName => "commands";
        public PluginHandler BackReference { get; set; }
        public MemberCode Rank => MemberCode.DEFAULT;
        public string MessagePrefix { get; set; }
        public string HelpFile => "Displays all available commands for you.";
        public ISender Sender { get; set; }
        public void SubscribeTo(ObservableHandler handler) { }

        public void OnPrivMsg(CommandMessage message) {
            var plugins = BackReference.plugins.OrderBy(s => s.Rank);
            var baserank = plugins.FirstOrDefault()?.Rank;
            string commands = $"\n{baserank}: ";

            foreach (IPluginContract pluginContract in plugins) {
                if (message.permission < pluginContract.Rank) {
                    break;
                }
                if (pluginContract.Rank == baserank) {
                    commands += pluginContract.CommandName + ", ";
                } else {
                    baserank = pluginContract.Rank;
                    commands += $"\n{baserank}: {pluginContract.CommandName}, ";
                }
            }
            commands = commands.Substring(0, commands.Length - 2);
            Sender.Send(RawNotice.Generate(
                message.user.userName,
                $"Following commands are available for you: {commands}")
            );
        }

        public void OnNotice(CommandMessage message) { }
    }
}
