using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using PluginContract;
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

        private IEnumerable<Lazy<IPluginContract>> xy;

        private readonly ChannelHandler _channelHandler;
        private readonly string path;

        public PluginHandler(ISender sender, ChannelHandler channelHandler, string pathToPlugins = "./../Plugins/") {
            this._sender = sender;
            path = pathToPlugins;
            // _catalog = new DirectoryCatalog(pathToPlugins);

            _channelHandler = channelHandler;
            LoadPlugins();
        }

        public void RefreshDirectory() {
            _catalog.Refresh();
        }

        public void ReleaseAll() {
            _container.ReleaseExports(xy);

            _container.Dispose();
            _catalog.Dispose();
            Debug.WriteLine("PLUGIN> Released all.");
        }

        public void LoadPlugins() {
            AppDomainSetup setup = new AppDomainSetup();
            setup.ApplicationName = "Plugins";
            setup.ShadowCopyFiles = "true";
            setup.CachePath = path + "cache/";
            AppDomain domain = AppDomain.CreateDomain("Plugins", AppDomain.CurrentDomain.Evidence, setup);
            PluginLoader pl = (PluginLoader) domain.CreateInstanceFromAndUnwrap("./Rabscuttle.exe", "Rabscuttle.core.handler.PluginLoader");
            pl.LoadPlugins();
            /*
            Assembly newAssembly = Assembly.LoadFrom("./Rabscuttle.exe");
            newAssembly.CreateInstance("Rabscuttle.core.handler.PluginLoader");

            AggregateCatalog catalog = new AggregateCatalog(new AssemblyCatalog(Assembly.GetExecutingAssembly()), new DirectoryCatalog(path));
            var files = Directory.GetFiles(path);
            foreach (string file in files) {
                if (!file.EndsWith(".dll")) {
                    continue;
                }

                Assembly assembly = Assembly.Load(System.IO.File.ReadAllBytes(Path.Combine(path, file)));
                catalog.Catalogs.Add(new AssemblyCatalog(assembly));
            }

            var x = 1 + 1;

            catalog.Dispose();
            var y = 1 + 1;
            /*
            _container = new CompositionContainer(_catalog);
            xy = _container.GetExports<IPluginContract>();
            // _container.ComposeParts(this);
            // Console.WriteLine($"PLUGIN> Loaded a total of {plugins.Length} plugins.");

            foreach (var lazy in xy) {
                Debug.WriteLine("PLUGIN> HUH? " + lazy.Value.CommandName);
            }

            foreach (IPluginContract plugin in plugins) {
                plugin.Sender = _sender;
                plugin.MessagePrefix = ">";
                Debug.WriteLine($"PLUGIN> Loaded '{plugin.CommandName}'");
            }
            */
        }



        public override void HandleCommand(NetworkMessage message) {
            throw new System.NotImplementedException();
        }

        public void HandleCommand(NetworkMessage message, UserRelation relation) {
            if (!message.message.StartsWith(prefix)) {
                return;
            }

            Debug.WriteLine("PLUGIN> Received:" + message);
            CommandMessage cmm = PrepareCommand(message);

            foreach (IPluginContract plugin in plugins) {

                if (plugin.CommandName == cmm.command && plugin.Rank <= relation.permission) {
                    plugin.OnPrivMsg(cmm);
                }
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
