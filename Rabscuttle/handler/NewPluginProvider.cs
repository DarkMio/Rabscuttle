using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using PluginContract;
using Rabscuttle.core.channel;
using Rabscuttle.core.io;

namespace Rabscuttle.core.handler {
    public class NewPluginProvider : MarshalByRefObject {

        private CompositionContainer container;
        private AssemblyCatalog catalog;
        // [Import(typeof(IPluginContract))]
        private IPluginContract export;
        public IPluginContract plugin => export;

        public void Compose(string filepath, string messagePrefix) {
            byte[] b = File.ReadAllBytes(filepath);
            var assembly = Assembly.Load(b);
            catalog = new AssemblyCatalog(assembly);
            container = new CompositionContainer(catalog);
            container.ComposeExportedValue(container);
            export = container.GetExportedValue<IPluginContract>();
            export.MessagePrefix = messagePrefix;
            // Debug.WriteLine("PLUGIN> {0} export in AppDomain {1}", export.CommandName, AppDomain.CurrentDomain.FriendlyName);
        }
    }
}
