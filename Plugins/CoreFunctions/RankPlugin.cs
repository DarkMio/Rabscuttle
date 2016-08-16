using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rabscuttle.handler;
using Rabscuttle.networking.commands;
using Rabscuttle.networking.io;
using Rabscuttle.plugins;

namespace CoreFunctions {
    [Export(typeof(IPluginContract))]
    public class RankPlugin : IPluginContract {
        public string CommandName => "rank";
        public PluginHandler BackReference { get; set; }
        public MemberCode Rank => MemberCode.DEFAULT;
        public string MessagePrefix { get; set; }
        public string HelpFile => "Gets your rank, as which the bot sees you.";
        public ISender Sender { get; set; }
        public void SubscribeTo(ObservableHandler handler) { }
        public void OnPrivMsg(CommandMessage message) {
            Sender.Send(RawNotice.Generate(message.user.userName, $"You are {message.permission}"));
        }
        public void OnNotice(CommandMessage message) {

        }
    }
}
