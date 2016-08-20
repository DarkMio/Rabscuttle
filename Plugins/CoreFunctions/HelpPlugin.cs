using System;
using Rabscuttle.handler;
using Rabscuttle.networking.commands;
using Rabscuttle.networking.io;
using Rabscuttle.plugins;

namespace CoreFunctions {
    public class HelpPlugin : IPluginContract {
        public string CommandName => "help";
        public PluginHandler BackReference { get; set; }
        public MemberCode Rank => MemberCode.DEFAULT;
        public string MessagePrefix { get; set; }
        public string HelpFile =>
                "This is the help file for the 'help' command." +
                $" You can query any help sending: \n{MessagePrefix}help <command>";
        public ISender Sender { get; set; }
        public void SubscribeTo(ObservableHandler handler) { }
        public void OnPrivMsg(CommandMessage message) {
            FindAndSendHelp(message);
        }
        public void OnNotice(CommandMessage message) {
            FindAndSendHelp(message);
        }

        /// <summary> Searches for the command and outputs its help file as NOTICE. </summary>
        /// <param name="message">The original command message.</param>
        private void FindAndSendHelp(CommandMessage message) {
            if (String.IsNullOrEmpty(message.parameters)) {
                Sender.Send(RawNotice.Generate(message.user.userName, HelpFile));
                return;
            }

            // split params, take first
            string command = message.parameters.Split(new []{' '}, 2)[0].ToLower();
            foreach (IPluginContract pluginContract in BackReference.plugins) {
                // if we would want to hide command help that the user wouldn't see, we could do that:
                //     message.permission >= pluginContract.Rank
                // but that#s an open design decision
                if (command == pluginContract.CommandName.ToLower() ) {
                    Sender.Send(RawNotice.Generate(message.user.userName, pluginContract.HelpFile));
                    return;
                }
            }

            Sender.Send(RawNotice.Generate(message.user.userName, "I could not find the command you're searching for."));
        }
    }
}
