﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Rabscuttle.channel;
using Rabscuttle.networking.commands;
using Rabscuttle.networking.io;
using Rabscuttle.plugin;
using Rabscuttle.util;

namespace Rabscuttle.handler {
    public class PluginHandler : ObservableHandler {
        [ImportMany(typeof(IPluginContract))]
        public List<IPluginContract> plugins = null;
        private readonly ISender _sender;
        private readonly string _prefix;
        public readonly ChannelHandler channelHandler;
        private readonly string _path;
        private readonly string[] _operators;
        private readonly VoidHandler _voidHandler;
        private readonly BanList _banlist;

        public PluginHandler(ISender sender, ChannelHandler channelHandler, VoidHandler voidHandler, string pathToPlugins = "./../Plugins/") {
            _sender = sender;
            _path = pathToPlugins;
            _prefix = ConfigurationProvider.Get("prefix");
            //_catalog = new DirectoryCatalog(pathToPlugins);
            _voidHandler = voidHandler;
            this.channelHandler = channelHandler;
            _banlist = BanList.Instance;

            var opList = ConfigurationProvider.Get("operators").Split(',');
            for (int index = 0; index < opList.Length; index++) {
                opList[index] = opList[index].Trim();
            }
            _operators = opList;

            plugins = new List<IPluginContract>();
            LoadPlugins();
        }

        private void LoadPlugins() {
            /*
            _catalog = new DirectoryCatalog(_path);
            _container = new CompositionContainer(_catalog);
            _container.ComposeParts(this);
            foreach (IPluginContract contract in plugins) {
                Console.WriteLine(contract.CommandName);
            }
            // var elements = _container.GetExportedValues<IPluginContract>();
            // _container.ComposeParts(this);
            */
            DirectoryInfo info = new DirectoryInfo("./../Plugins/");
            if (!info.Exists) {
                Logger.WriteWarn("Plugin Handler", "Plugin Folder not found!");
                return;
            }

            foreach (FileInfo file in info.GetFiles("*.dll")) {

                Assembly a = Assembly.LoadFrom(file.FullName);
                Type[] types = a.GetExportedTypes();
                foreach (Type t in types) {
                    if (!typeof(IPluginContract).IsAssignableFrom(t)) {
                        continue;
                    }
                    try {
                        var instance = Activator.CreateInstance(t) as IPluginContract;
                        if (instance != null) { // if it's really, really part of the plugin classing...
                            plugins.Add(instance);
                        }
                    } catch (Exception e) when (e is ArgumentException || e is MissingMethodException) {
                        Logger.WriteWarn("Plugin Handler", $"Could not load {file.Name}, caused by: {e.Message}");
                    }
                }
            }
            Logger.WriteInfo("Plugin Handler", "Loaded a total of {0} plugins.", plugins.Count);



            foreach (IPluginContract plugin in plugins) {
                plugin.Sender = _sender;
                plugin.MessagePrefix = _prefix;
                plugin.BackReference = this;
                plugin.SubscribeTo(_voidHandler); // currently I don't have much more to subscribe to
                Logger.WriteDebug("Plugin Handler", "Loaded: [ {0} ]", plugin.CommandName);
            }
        }



        public override void HandleCommand(NetworkMessage message) {
            //@TODO: Does not take care about bot _operators.
            CommandCode type = (CommandCode) message.typeEnum;
            if (type != CommandCode.PRIVMSG && type != CommandCode.NOTICE) {
                return;
            }
            if (!message.message.StartsWith(_prefix)) {
                return;
            }

            CommandMessage cmsg = PrepareCommand(message);
            bool ban = _banlist.CheckBan(cmsg.user);
            if (_banlist.CheckBan(cmsg.user) && !CheckForBotoperator(cmsg.user)) {
                Logger.WriteInfo("Plugin Handler", "Ignoring command [ {0} ] from user [ {1} ]", cmsg.command, cmsg.user.userName);
                return;
            }
            HandleCommand(cmsg);
        }

        private void HandleCommand(CommandMessage message) {
            foreach (IPluginContract plugin in plugins) {
                if (plugin.CommandName != message.command) {
                    continue;
                }

                // if the rank outweights the users permission, we may check for his bot-operator status.
                if (plugin.Rank > message.permission) {
                    if (!CheckForBotoperator(message.user)) {
                        continue;
                    }
                    message.permission = MemberCode.BOTOPERATOR;
                }

                Logger.WriteDebug("Plugin Handler", "Handling message: {0}", message.message);
                CommandCode type = (CommandCode) message.message.typeEnum;
                if (type == CommandCode.PRIVMSG) {
                    plugin.OnPrivMsg(message);
                } else {
                    plugin.OnNotice(message);
                }
                return;
            }
        }

        public override void HandleReply(NetworkMessage message) {
            throw new System.NotImplementedException();
        }

        public IPluginContract FindPlugin(string name) {
            return plugins.SingleOrDefault(s => s.CommandName == name);
        }

        private bool CheckForBotoperator(ChannelUser user) {
            Task<bool> task = channelHandler.IsLoggedIn(user.userName);
            task.Wait();
            return _operators.Any(s => s.Trim() == user.loginUserName);
        }

        private bool SortOutMessage(NetworkMessage message, UserRelation relation) {
            //@TODO: Once I've come around to write the ban plugin, this will be checking for it
            return false;
        }

        /// <summary> Converts a <see cref="NetworkMessage"/> to a <see cref="CommandMessage"/> and searches for all relevant data. </summary>
        /// <param name="message">A NetworkMessage that has to be of Type <c>NOTICE</c> or <c>PRIVMSG</c>.</param>
        /// <returns> A CommandMessage which can be sent to plugins. They contain all relevant data. </returns>
        private CommandMessage PrepareCommand(NetworkMessage message) {
            CommandCode type = (CommandCode) message.typeEnum;
            if (type != CommandCode.PRIVMSG && type != CommandCode.NOTICE) { // the message is of an unsupported Type
                var error =
                    $"The given NetworkMessage is not of type [ NOTICE ] or [ PRIVMSG ], instead is [ {message.type} ].";
                Logger.WriteError("Plugin Handler", error);
                throw new ArgumentException(error);
            }

            string[] split = message.message.Split(new[] {' '}, 2); // first is command, second are all parameter
            string args = "";
            string command = split[0].Substring(_prefix.Length); // cut off the prefix
            if (split.Length > 1) {
                args = split[1];
            }

            ChannelUser user = channelHandler.FindUser(message.prefix) ?? new ChannelUser(message.prefix);
            // typeParams can be a channel or a user
            // @TODO: What happens when username and channel name are same?
            Channel chan = channelHandler.FindChannel(message.typeParams);
            MemberCode permission = MemberCode.DEFAULT;


            if (_operators.Any(s => user.userName == s) && CheckForBotoperator(user)) { // when username in _operators and IS an operator
                permission = MemberCode.BOTOPERATOR;
            } else if (chan != null) { // otherwise, if the channel is not null, give us the permission in his channel
                UserRelation relation = chan.users.SingleOrDefault(s => s.user.userName == user.userName);
                if (relation != null) {
                    permission = relation.Permission;
                } else { // uh oh, that went wrong - user not found in any channels
                    Logger.WriteWarn("Plugin Handler", "User [ {0} ] requested a command but was not found in the userlists.", user.userName);
                }
            }

            // Assemble the final command message.
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
