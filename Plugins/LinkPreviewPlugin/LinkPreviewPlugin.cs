using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rabscuttle.handler;
using Rabscuttle.networking.commands;
using Rabscuttle.networking.io;
using Rabscuttle.plugin;

namespace LinkPreviewPlugin {
    public class LinkPreviewPlugin : IPluginContract, IObserver<NetworkMessage> {
        public string CommandName => "linkpreview";
        public PluginHandler BackReference { get; set; }
        public MemberCode Rank => MemberCode.BOTOPERATOR;
        public string MessagePrefix { get; set; }
        public string HelpFile
            => "The link previewer subscribes to channels and sends appropiate link-previews when available.";
        public ISender Sender { get; set; }
        private readonly LinkCoordinator _coordinator = LinkCoordinator.Instance;
        public void SubscribeTo(ObservableHandler handler) {
            VoidHandler voidHandler = handler as VoidHandler;
            voidHandler?.Subscribe(this);
        }

        public void OnPrivMsg(CommandMessage message) { }
        public void OnNotice(CommandMessage message) { }
        public void OnNext(NetworkMessage value) {
            if ((CommandCode) value.typeEnum == CommandCode.PRIVMSG) {
                string response = _coordinator.GenerateResponse(value.message);
                if (response != null) {
                    Sender.Send(RawPrivMsg.Generate(value.typeParams, response));
                }
            }
        }

        public void OnError(Exception error) { }
        public void OnCompleted() { }
    }
}
