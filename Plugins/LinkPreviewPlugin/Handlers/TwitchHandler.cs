using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LinkPreviewPlugin.Handlers {
    class TwitchHandler : ILinkHandler {

        private readonly Regex _regex =
            new Regex(
                @"(?:^http(?:s)?://)? (?# http, https)
                    (?:               (?# then there might be a bunch of subdomains)
                      (?:
                        www|en-es|en-gb|secure|beta|ro|www-origin|
                        en-ca|fr-ca|lt|zh-tw|he|id|ca|mk|lv|ma|tl|
                        hi|ar|bg|vi|th|clips
                      )\.)?
                    twitch.tv/         (?# domain iteself)
                    (?!                (?# neg lookahead on some non-channel locations)
                      directory|p|user/legal|admin|
                      login|signup|jobs
                    )
                    (?<channel>\w+)    (?# the channel name itself)
                    (?:
                      (?:/v)?/
                      (?<clip>\w+)     (?# and then either a clip or vod url)
                    )?",
                RegexOptions.IgnorePatternWhitespace
            );

        public string GenerateResponse(string url) {
            var matches = _regex.Matches(url)[0].Groups;
            var channel = matches["channel"].Value;
            var clip = matches["clip"].Value;
            if (!String.IsNullOrWhiteSpace(clip)) {
                return null;
            }

            JObject response = Utility.GetFromApi<JObject>($"https://api.twitch.tv/kraken/streams/{channel}");
            bool online = true;
            long viewers = 0;
            JObject channelInfo;
            if (response["stream"].Type != JTokenType.Null) { // channel is online!
                viewers = response["stream"]["viewers"].Value<long>();
                channelInfo = response["stream"]["channel"].Value<JObject>();
            } else { // channel is offline then
                channelInfo = Utility.GetFromApi<JObject>($"https://api.twitch.tv/kraken/channels/{channel}");
                online = false;
            }

            string prequel = online ? "[LIVE] " : "";
            string views = online ? $"👁{viewers:N0} | " : "";
            string streamer = channelInfo["display_name"].Value<string>();
            long followers = channelInfo["followers"].Value<long>();
            string streamTitle = channelInfo["status"].Value<string>();
            string output = $"{prequel}{streamer} ({views}{followers:N0}🖤): {streamTitle}";

            return output;
        }

        public bool ReactsToUrl(string url) {
            return url.Contains("twitch.tv");
        }
    }
}
