﻿using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Reflection;

namespace Rabscuttle.stuff {
    public class ConfigurationProvider {
        private static readonly Configuration CONFIG =
            ConfigurationManager.OpenExeConfiguration(Assembly.GetEntryAssembly().Location);
        private static readonly KeyValueConfigurationCollection COLLECTION = CONFIG.AppSettings.Settings;

        public static void Bootstrap() {
            GetOrSetDefault("network", "");
            GetOrSetDefault("port", "6667");
            GetOrSetDefault("username", "Rabscuttle"); // username of the bot
            GetOrSetDefault("realname", "Mr. Robit"); // realname string of IRC
            GetOrSetDefault("operators", ""); // comma seperated user names
            GetOrSetDefault("prefix", ">");
            GetOrSetDefault("channels", ""); // comma seperated autojoining channels
            GetOrSetDefault("loglevel", "info");
            GetOrSetDefault("bootcommands", ""); // comma seperated commands which will be sent first


        }

        public static string Get(string key) {
            return COLLECTION[key].Value;
        }

        private static string GetOrSetDefault(string key, string defaultValue) {
            if (COLLECTION[key] == null) {
                Logger.WriteDebug("Config Manager", "Should add?!");
                COLLECTION.Add(key, defaultValue);
                CONFIG.Save(ConfigurationSaveMode.Modified);
            }
            return COLLECTION[key].ToString();
        }
    }
}
