using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using NUnit.Framework;
using NUnit.Framework.Api;
using Rabscuttle.core.io;

namespace Rabscuttle.core.handler {
    public class NewPluginHandler : ObservableHandler {

        private readonly string shadowCopyPath;
        private readonly string pluginPath;

        static void Main(string[] args) {
            for (int i = 0; i < 5; i++) {
                Thread.Sleep(1500);
            }
            /*
            ReadDLL(@"E:\Projects\c#\Rabscuttle\Rabscuttle\bin\Debug\Plugins\FuckPlugin.dll");
            ReadDLL(@"E:\Projects\c#\Rabscuttle\Rabscuttle\bin\Debug\Plugins\ExamplePlugin.dll");

            Console.WriteLine("Done.");

            while (true) {
                Thread.Sleep(1500);
            }
            */

            Debug.WriteLine("First fucking loading.");
            Thread.Sleep(5000);

            List<AppDomain> list = new List<AppDomain>();
            for(int i = 0; i < 150; i++) {
                list.Add(PluginDomainFactory());

            }


            Debug.WriteLine("All loaded u bitch.");
            Thread.Sleep(5000);

            foreach (AppDomain a in list) {
                AppDomain.Unload(a);
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }



            Debug.WriteLine("D O N E .");

            while (true) {
                Thread.Sleep(1500);
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }

        }

        public NewPluginHandler(string shadowCopyPath = "./_cache/", string pluginPath = "./../Plugins") {
            this.shadowCopyPath = shadowCopyPath;
            this.pluginPath = pluginPath;
        }

        public static void ReadDLL(string filePath) {
            byte[] b = File.ReadAllBytes(filePath);
            var assembly = Assembly.Load(b);

        }

        public static AppDomain PluginDomainFactory() {
            string shadowCopyPath = "./_cache/";
            string pluginPath = "./../Plugins";
            if (!Directory.Exists(shadowCopyPath)) {
                Directory.CreateDirectory(shadowCopyPath);
            }

            if (!Directory.Exists(pluginPath)) {
                Directory.CreateDirectory(pluginPath);
            }

            var setup = new AppDomainSetup {
                CachePath = shadowCopyPath,
                ShadowCopyFiles = "true",
                LoaderOptimization = LoaderOptimization.MultiDomainHost,
                ShadowCopyDirectories = pluginPath // minimize the hdd overhead by just copying the important stuff
            };

            var newDomain = AppDomain.CreateDomain("Host_AppDomain", AppDomain.CurrentDomain.Evidence, setup);
            var runner =
                (NewPluginRunner)
                    newDomain.CreateInstanceAndUnwrap(typeof(NewPluginRunner).Assembly.FullName,
                        typeof(NewPluginRunner).FullName
                    );

            Console.WriteLine("The main AppDomain is: {0}", AppDomain.CurrentDomain.FriendlyName);

            runner.DoSomething(newDomain.FriendlyName);
            return newDomain;
        }

        public override void HandleCommand(NetworkMessage message) {
            throw new System.NotImplementedException();
        }

        public override void HandleReply(NetworkMessage message) {
            throw new System.NotImplementedException();
        }
    }
}
