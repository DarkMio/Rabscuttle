using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using PluginContract;
using Rabscuttle.channel;
using Rabscuttle.core.channel;
using Rabscuttle.core.io;
using Rabscuttle.core.commands;
using Rabscuttle.stuff;

namespace Rabscuttle.core.handler {
    public class PluginHandler : ObservableHandler {
        [ImportMany(typeof(IPluginContract), AllowRecomposition = true)]
        public IPluginContract[] plugins = null;
        private readonly ISender _sender;
        private readonly DirectoryCatalog _catalog;
        private CompositionContainer _container;
        private readonly string prefix = ">";
        public readonly ChannelHandler channelHandler;
        private readonly string path;
        private readonly string[] operators;

        public PluginHandler(ISender sender, ChannelHandler channelHandler, string pathToPlugins = "./../Plugins/") {
            this._sender = sender;
            path = pathToPlugins;
            _catalog = new DirectoryCatalog(pathToPlugins);

            this.channelHandler = channelHandler;

            var opList = ConfigurationProvider.Get("operators").Split(',');
            for (int index = 0; index < opList.Length; index++) {
                opList[index] = opList[index].Trim();
            }
            operators = opList;

            LoadPlugins();
        }

        private void LoadPlugins() {
            _container = new CompositionContainer(_catalog);
            //plugins = _container.GetExportedValues<IPluginContract>();
            _container.ComposeParts(this);
            Logger.WriteInfo("Plugin Handler", "Loaded a total of {0} plugins.", plugins.Length);

            foreach (IPluginContract plugin in plugins) {
                plugin.Sender = _sender;
                plugin.MessagePrefix = prefix;
                plugin.BackReference = this;
                Logger.WriteDebug("Plugin Handler", "Loaded {0}", plugin.CommandName);
            }
        }



        public override void HandleCommand(NetworkMessage message) {
            //@TODO: Does not take care about bot operators.
            if ((CommandCode) message.typeEnum != CommandCode.PRIVMSG) {
                return;
            }

            if (!message.message.StartsWith(prefix)) {
                return;
            }

            CommandMessage cmsg = PrepareCommand(message);
            Channel chan = channelHandler.FindChannel(cmsg.origin);
            if (chan == null) { // so the origin is a user
                HandleCommand(cmsg, MemberCode.DEFAULT);
                return;
            }

            UserRelation relation = chan.users.SingleOrDefault(s => s.user.userName == cmsg.user.userName);
            if (relation == null) {
                Logger.WriteWarn("Plugin Handler", "No viable user found.");
                HandleCommand(cmsg, MemberCode.DEFAULT);
                return;
            }

            HandleCommand(cmsg, relation.permission);
        }

        public void HandleCommand(CommandMessage message, MemberCode rank) {
            // Debug.WriteLine("PLUGIN> Received:" + message);
            // CommandMessage cmm = PrepareCommand(message);
            foreach (IPluginContract plugin in plugins) {
                if (plugin.Rank == MemberCode.BOTOPERATOR && CheckForBotoperator(message.user)) {

                } else if (plugin.CommandName != message.command || plugin.Rank > rank) {
                    continue;
                }

                Logger.WriteDebug("Plugin Handler", "Handling message: {0}", message.message);
                plugin.OnPrivMsg(message);
            }
        }

        public void HandleCommand(NetworkMessage message, UserRelation relation) {
            if (!message.message.StartsWith(prefix)) {
                return;
            }

            if (SortOutMessage(message, relation)) {
                return;
            }


            Debug.WriteLine("PLUGIN> Received:" + message);
            CommandMessage cmm = PrepareCommand(message);

            foreach (IPluginContract plugin in plugins) {
                if (CheckForBotoperator(relation.user)) {
                    plugin.OnPrivMsg(cmm);
                    continue;
                }
                if (CheckForBotoperator(relation.user)) {
                    plugin.OnPrivMsg(cmm);
                    continue;
                }
                if (plugin.CommandName != cmm.command || plugin.Rank > relation.permission) {
                    continue;
                }
                plugin.OnPrivMsg(cmm);
            }
        }

        public override void HandleReply(NetworkMessage message) {
            throw new System.NotImplementedException();
        }

        public IPluginContract FindPlugin(string name) {
            return plugins.SingleOrDefault(s => s.CommandName == name);
        }

        private bool CheckForBotoperator(ChannelUser user) {
            if (operators.Any(s => s.Trim() == user.userName)) {
                Task<bool> task = channelHandler.IsLoggedIn(user.userName);
                Logger.WriteInfo("Plugin Handler", "Searching for bot operator rights on {0}", user.userName);
                task.Wait();
                Logger.WriteDebug("Plugin Handler", "Operator Rights are: {0}", task.Result);
                return task.Result;
            }
            return false;
        }

        private bool SortOutMessage(NetworkMessage message, UserRelation relation) {
            //@TODO: Once I've come around to write the ban plugin, this will be checking for it
            return false;
        }

        private CommandMessage PrepareCommand(NetworkMessage message) {
            string[] split = message.message.Split(new[] {' '}, 2);
            string args = "";
            string command = split[0].Substring(prefix.Length);
            if (split.Length > 1) {
                args = split[1];
            }
            ChannelUser user = new ChannelUser(message.prefix);
            Channel chan = channelHandler.FindChannel(message.typeParams);
            UserRelation relation = null;
            MemberCode permission = MemberCode.DEFAULT;

            if (operators.Any(s => user.userName == s) && CheckForBotoperator(user)) {
                permission = MemberCode.BOTOPERATOR;
            } else if (chan != null) {
                relation = chan.users.SingleOrDefault(s => s.user.userName == message.prefix.Split(new[] {'!'}, 2)[0]);
                permission = relation.permission;
            }


            CommandMessage cmm = new CommandMessage() {
                message = message,
                parameters = args,
                command = command,
                origin = message.typeParams,
                user = user,
                permission = permission
            };

            return cmm;
        }
    }
}
