using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Rabscuttle.handler;
using Rabscuttle.networking.commands;
using Rabscuttle.networking.io;
using Rabscuttle.plugin;

namespace ModdotaAnnouncerPlugin {
    public class ModdotaAnnouncerPlugin : IPluginContract {
        public string CommandName => "announcer";
        public PluginHandler BackReference { get; set; }
        public MemberCode Rank => MemberCode.BOTOPERATOR;
        public string MessagePrefix { get; set; }
        public string HelpFile => "Starts the ModDota Forums Announcer.";
        public ISender Sender { get; set; }
        public void SubscribeTo(ObservableHandler handler) { }

        // in case I want to extend the functionality.
        private static readonly List<AnnouncerProcess> THREADS = new List<AnnouncerProcess>();
        private class AnnouncerProcess {
            public string channel;
            public Thread thread;
        }

        public void OnPrivMsg(CommandMessage message) {
            if (message.parameters.ToLower().StartsWith("kill")) {
                foreach (AnnouncerProcess announcerProcess in THREADS) {
                    announcerProcess.thread.Abort();
                }
                THREADS.Clear();
                Sender.Send(RawPrivMsg.Generate(message.origin, "Killed all announcer threads."));
                return;
            }
            if (THREADS.SingleOrDefault(c => c.channel == message.origin) != null) {
                Sender.Send(RawPrivMsg.Generate(message.origin, "The announcer is already running in this channel."));
                return;
            }
            DatabaseManager announcer = new DatabaseManager(Sender);
            Thread thread = new Thread(() => { announcer.Run(message.origin); });
            thread.IsBackground = true;
            thread.Start();
            THREADS.Add(new AnnouncerProcess {channel = message.origin, thread = thread});
            Sender.Send(RawPrivMsg.Generate(message.origin, "Announcer started."));
        }

        public void OnNotice(CommandMessage message) { }
    }
}
