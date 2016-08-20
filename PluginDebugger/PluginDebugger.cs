using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Rabscuttle.networking.commands;
using Rabscuttle.plugins;

namespace PluginDebugger {
    class Debugger {
        static void Main(string[] args) {
            var cmd = new CommandMessage {command = "updated", origin = "#dota2mods", parameters = "DotaCraft"};
            new GithubUpdatedPlugin.GithubUpdatedPlugin().OnPrivMsg(cmd);

            /*
             *
            var jo = KarmaPlugin.KarmaPlugin.ReadAndParseFile("report.json");
            foreach (KeyValuePair<string, JToken> keyValuePair in jo) {
                Debug.WriteLine($"{keyValuePair.Key}: {keyValuePair.Value}");
            }
            *
            */
        }
    }
}
