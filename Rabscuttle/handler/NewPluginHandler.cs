using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using NUnit.Framework;
using NUnit.Framework.Api;
using PluginContract;
using Rabscuttle.channel;
using Rabscuttle.core.channel;
using Rabscuttle.core.commands;
using Rabscuttle.core.io;

namespace Rabscuttle.core.handler {
    public class NewPluginHandler : ObservableHandler {

        private readonly string shadowCopyPath;
        private readonly string pluginPath;
        private readonly List<NewPluginProvider> pluginStorages;
        private readonly FileSystemWatcher watcher;
        private readonly string _prefix;
        private readonly ISender _sender;
        private readonly ChannelHandler _channelHandler;

        class PluginStorage {
            public readonly AppDomain pluginDomain;
            public readonly NewPluginProvider provider;
            public readonly IPluginContract plugin;

            public PluginStorage(AppDomain pluginDomain, IPluginContract plugin, NewPluginProvider provider) {
                this.pluginDomain = pluginDomain;
                this.plugin = plugin;
                this.provider = provider;
            }
        }

        public static void Main(string[] args) {
            new NewPluginHandler(">", null, null);
        }

        public NewPluginHandler(string prefix, ISender sender, ChannelHandler channelHandler, string shadowCopyPath = "./_cache/", string pluginPath = "./../Plugins") {
            _channelHandler = channelHandler;
            _prefix = prefix;
            _sender = sender;
            this.shadowCopyPath = shadowCopyPath;
            this.pluginPath = pluginPath;
            pluginStorages = new List<NewPluginProvider>();
            GenerateFolders();
            watcher = new FileSystemWatcher(pluginPath, "*.dll");
            // watch for: Changes in write-time / creation-time, in size or if the filename has changed
            watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size | NotifyFilters.FileName | NotifyFilters.CreationTime;
            watcher.Changed += OnChanged;
            watcher.EnableRaisingEvents = true;

            InitializePlugins();
        }

        private void OnChanged(object source, FileSystemEventArgs e) {

        }

        private void GenerateFolders() {
            if (!Directory.Exists(shadowCopyPath)) {
                Directory.CreateDirectory(shadowCopyPath);
            }

            if (!Directory.Exists(pluginPath)) {
                Directory.CreateDirectory(pluginPath);
            }
        }

        private void InitializePlugins() {
            var files = Directory.EnumerateFiles(pluginPath, "*.dll");
            foreach (string filePath in files) {
                Debug.WriteLine(filePath);
                ComposePlugin(filePath);
            }
        }


        private void ComposePlugin(string filePath) {
            var setup = new AppDomainSetup {
                CachePath = shadowCopyPath,
                ShadowCopyFiles = "true",
                LoaderOptimization = LoaderOptimization.MultiDomain,
                ShadowCopyDirectories = pluginPath // minimize the hdd overhead by just copying the important stuff
            };

            var newDomain = AppDomain.CreateDomain(System.Guid.NewGuid().ToString(), AppDomain.CurrentDomain.Evidence, setup);
            var runner =
                (NewPluginProvider)
                    newDomain.CreateInstanceAndUnwrap(typeof(NewPluginProvider).Assembly.FullName,
                        typeof(NewPluginProvider).FullName
                    );

            Debug.WriteLine("PLUGIN> The main AppDomain is: {0}", AppDomain.CurrentDomain.FriendlyName);
            byte[] b = File.ReadAllBytes(filePath);
            runner.Compose(filePath, _prefix);


            // PluginStorage ps = new PluginStorage(newDomain, runner.plugin, runner);
            pluginStorages.Add(runner);
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

            foreach (NewPluginProvider pp in pluginStorages) {

                if (pp.plugin.CommandName == message.command && pp.plugin.Rank <= rank) {
                    Debug.WriteLine("Fuck this.");
                    NetworkMessage nm = pp.plugin.OnPrivMsg(message);
                    if (nm == null) {
                        continue;
                    }
                    _sender.Send(nm);

                }
            }
        }

        public override void HandleReply(NetworkMessage message) {
            throw new System.NotImplementedException();
        }


        public CommandMessage PrepareCommand(NetworkMessage message) {
            string[] split = message.message.Split(new[] {' '}, 2);
            string args = "";
            string command = split[0].Substring(_prefix.Length);
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
}
