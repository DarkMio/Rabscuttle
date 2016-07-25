using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using PluginContract;
using Rabscuttle.networking.commands;
using Rabscuttle.networking.io;

namespace Rabscuttle.networking.handler {
    public class PluginHandler : ObservableHandler {

        [ImportMany(typeof(IPluginContract))]
        private IPluginContract[] plugins = null;

        private ISender sender;
        private DirectoryCatalog catalog;

        public PluginHandler(ISender sender, string pathToPlugins = "./Plugins/") {
            this.sender = sender;
            catalog = new DirectoryCatalog(pathToPlugins);
            LoadPlugins();
        }

        public void LoadPlugins() {
            var container = new CompositionContainer(catalog);
            container.ComposeParts(this);
            Console.WriteLine($"PLUGIN> Loaded a total of {plugins.Length} plugins.");
            foreach (IPluginContract plugin in plugins) {
                plugin.Sender = sender;
                plugin.MessagePrefix = ">";
            }
        }

        public override void HandleCommand(NetworkMessage message) {

            Debug.WriteLine("PLUGIN> Received:" + message);
            /*
            if ((CommandCode) message.typeEnum != CommandCode.PRIVMSG) {
                return;
            }
            */
            var commandName = message.message.Split(' ')[0].Replace(">", "");
            Debug.WriteLine(commandName);
            foreach (IPluginContract plugin in plugins) {
                Debug.WriteLine(plugin.CommandName);
                if (plugin.CommandName == commandName) {
                    plugin.OnPrivMsg(message);
                }
            }
        }

        public override void HandleReply(NetworkMessage message) {
            throw new System.NotImplementedException();
        }
    }
}
