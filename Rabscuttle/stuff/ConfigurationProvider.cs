using System.Collections.Specialized;
using System.Configuration;

namespace Rabscuttle.stuff {
    public class ConfigurationProvider {
        private static NameValueCollection config = ConfigurationManager.AppSettings;

        public static void Bootstrap() {
            GetOrSetDefault("network", "");
            GetOrSetDefault("port", "6667");
            GetOrSetDefault("username", "Rabscuttle"); // username of the bot
            GetOrSetDefault("realname", "Mr. Robit"); // realname string of IRC
            GetOrSetDefault("operators", ""); // comma seperated user names
            GetOrSetDefault("prefix", ">");
            GetOrSetDefault("channels", ""); // comma seperated autojoining channels
            GetOrSetDefault("loglevel", "info");
        }

        private static string GetOrSetDefault(string key, string defaultValue) {
            if (config["key"] == null) {
                config["key"] = defaultValue;
            }
            return config["key"];
        }
    }
}
