using System;
using Rabscuttle.handler;
using Rabscuttle.networking.commands;
using Rabscuttle.networking.io;
using Rabscuttle.plugin;

namespace RandomToolsCrashPlugin {
    public class RandomToolsCrashPlugin : IPluginContract {
        public string CommandName => "rtc";
        public PluginHandler BackReference { get; set; }
        public MemberCode Rank => MemberCode.DEFAULT;
        public string MessagePrefix { get; set; }
        public string HelpFile => "Whenever your tools crash, call this plugin. It keeps track of it.";
        public ISender Sender { get; set; }
        public void SubscribeTo(ObservableHandler handler) {}

        private readonly string[] _intros = new[] {
            "Good job!", "Yay,", "Woohoo,", "Someone tell valve:", "Great,",
            "ffs", "c'mon", "C'M-FUCKING-ON,", "Jesses Maria Cruzifix,", "Blame SQL, but a",
            "Wow! Amazing!", "Jesustittychrist,", "Fuck this shit -", "", "yuck",
        };

        private readonly Random _seed = new Random();

        public void OnPrivMsg(CommandMessage message) {
            CrashManager.Instance.Add();
            var stats = CrashManager.Instance.GetStats();
            string intro = _intros[_seed.Next(_intros.Length)];
            Sender.Send(RawPrivMsg.Generate(
                message.origin,
                $"{intro} random tools crash! (🗓 {stats.week} | ∑{stats.total + stats.week})")
            );
        }

        public void OnNotice(CommandMessage message) {}
    }
}
