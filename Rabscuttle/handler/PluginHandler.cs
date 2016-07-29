using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using PluginContract;
using Rabscuttle.channel;
using Rabscuttle.core.channel;
using Rabscuttle.core.io;
using Rabscuttle.core.commands;

namespace Rabscuttle.core.handler {
    public class PluginHandler : ObservableHandler {

        [ImportMany(typeof(IPluginContract), AllowRecomposition = true)]
        private IPluginContract[] plugins = null;

        private readonly ISender _sender;
        private readonly DirectoryCatalog _catalog;
        private CompositionContainer _container;
        private readonly string prefix = ">";


        private readonly ChannelHandler _channelHandler;
        private readonly string path;

        public PluginHandler(ISender sender, ChannelHandler channelHandler, string pathToPlugins = "./../Plugins/") {
            this._sender = sender;
            path = pathToPlugins;
            _catalog = new DirectoryCatalog(pathToPlugins);

            _channelHandler = channelHandler;
            LoadPlugins();
        }

        public void LoadPlugins() {

            _container = new CompositionContainer(_catalog);
            //plugins = _container.GetExportedValues<IPluginContract>();
            _container.ComposeParts(this);
            Console.WriteLine($"PLUGIN> Loaded a total of {plugins.Length} plugins.");

            foreach (IPluginContract plugin in plugins) {
                plugin.Sender = _sender;
                plugin.MessagePrefix = ">";
                Debug.WriteLine($"PLUGIN> Loaded '{plugin.CommandName}'");
            }

        }



        public override void HandleCommand(NetworkMessage message) {
            //@TODO: Does not take care about bot operators.
            if ((CommandCode) message.typeEnum != CommandCode.PRIVMSG) {
                return;
            }

            if (!message.message.StartsWith(">")) {
                Debug.WriteLine("PLUGIN> Not for any plugin: " + message.message);
                return;
            }

            Debug.WriteLine("PLUGIN> Received: " + message.message);


            CommandMessage cmsg = PrepareCommand(message);
            Channel chan = _channelHandler.FindChannel(cmsg.origin);
            if (chan == null) { // so the origin is a user
                HandleCommand(cmsg, MemberCode.DEFAULT);
                return;
            }

            UserRelation relation = chan.users.SingleOrDefault(s => s.user == cmsg.user);
            if (relation == null) {
                Debug.WriteLine("PLUGIN> WARNING: User not found.");
                HandleCommand(cmsg, MemberCode.DEFAULT);
                return;
            }

            HandleCommand(cmsg, relation.permission);
        }

        public void HandleCommand(CommandMessage message, MemberCode rank) {
            /*if (!message.command.StartsWith(">")) {
                return;
            }
            */
            Debug.WriteLine("PLUGIN> Received:" + message);
            // CommandMessage cmm = PrepareCommand(message);

            foreach (IPluginContract plugin in plugins) {

                if (plugin.CommandName == message.command && plugin.Rank <= rank) {
                    Debug.WriteLine("Fuck this.");
                    NetworkMessage nm = plugin.OnPrivMsg(message);
                    if (nm == null) {
                        continue;
                    }
                    _sender.Send(nm);

                }
            }
        }

        public void HandleCommand(NetworkMessage message, UserRelation relation) {
            if (!message.message.StartsWith(prefix)) {
                return;
            }

            Debug.WriteLine("PLUGIN> Received:" + message);
            CommandMessage cmm = PrepareCommand(message);

            foreach (IPluginContract plugin in plugins) {
                if (plugin.CommandName != cmm.command || plugin.Rank > relation.permission) {
                    continue;
                }
                NetworkMessage nm = plugin.OnPrivMsg(cmm);
                if (nm == null) {
                    continue;
                }
                _sender.Send(nm);
            }
        }

        public override void HandleReply(NetworkMessage message) {
            throw new System.NotImplementedException();
        }

        public CommandMessage PrepareCommand(NetworkMessage message) {
            string[] split = message.message.Split(new[] {' '}, 2);
            string args = "";
            string command = split[0].Substring(prefix.Length);
            if (split.Length > 1) {
                args = split[1];
            }

            CommandMessage cmm = new CommandMessage() {
                message = message,
                parameters = args,
                command = command,
                origin = message.typeParams,
                user = new ChannelUser(message.prefix)
            };

            return cmm;
        }
    }

    [Serializable]
    public class PluginLoader {

        [ImportMany(typeof(IPluginContract))]
        private IPluginContract[] plugins = null;


        public void LoadPlugins() {
            var aggregateCatalog = new AggregateCatalog();
            aggregateCatalog.Catalogs.Add(new DirectoryCatalog(@"E:\Projects\c#\Rabscuttle\Rabscuttle\bin\Plugins"));
            var container = new CompositionContainer(aggregateCatalog);
            container.ComposeParts(this);
            foreach (IPluginContract plugin in plugins) {
                // plugin.Sender = _sender;
                plugin.MessagePrefix = ">";
                Console.WriteLine($"PLUGIN> Loaded '{plugin.CommandName}'");
            }
        }
    }
}
