using System;
using Rabscuttle.handler;
using Rabscuttle.networking.commands;
using Rabscuttle.networking.io;
using Rabscuttle.plugin;

namespace SteamPlugins {
    public class SteamRestartPlugin : IPluginContract {
        public string CommandName => "steamrestart";
        public PluginHandler BackReference { get; set; }
        public MemberCode Rank => MemberCode.DEFAULT;
        public string MessagePrefix { get; set; }
        public string HelpFile => "Replies with the time taken until the steam store restarts.";
        public ISender Sender { get; set; }
        public void SubscribeTo(ObservableHandler handler) { }
        public void OnPrivMsg(CommandMessage message) {
            // if cancer didn't eat me away, someone has to tell me to update this, then.
            TimeSpan span = new DateTime(2100, 1, 1, 17, 0, 0, DateTimeKind.Utc) - DateTime.UtcNow;
            Sender.Send(RawPrivMsg.Generate(message.origin, $"The Steam Store resets in {span.Hours}h {span.Minutes}m."));
        }
        public void OnNotice(CommandMessage message) { }
    }
}
