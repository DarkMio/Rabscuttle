using System;
using System.IO;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Authenticators;

namespace LinkPreviewPlugin.Handlers {
    public class TwitterHandler : ILinkHandler {
        private const string Path = "../Plugins/PluginData/";
        private readonly Regex _regex = new Regex(@"(^|/|\.)twitter\.com/(.+?)/status/(?<id>[0-9]+)/?");
        private readonly string _key;
        private readonly string _secret;
        private readonly string _access;
        private readonly string _accessSecret;

        public TwitterHandler() {
            using (StreamReader s = new StreamReader(Path + "twitter.key")) {
                _key = s.ReadLine();
                _secret = s.ReadLine();
                _access = s.ReadLine();
                _accessSecret = s.ReadLine();
            }
        }

        public string GenerateResponse(string url) {
            string match = _regex.Matches(url)[0].Groups["id"].Value;
            JObject jObject = GetStatus(match);
            string user = jObject["user"]["screen_name"].Value<string>();
            string message = jObject["text"].Value<string>();
            return $"@{user} tweeted: {message}";
        }

        private JObject GetStatus(string statusId) {
            Uri baseUri = new Uri("https://api.twitter.com");

            RestClient client = new RestClient(baseUri) {
                Authenticator = OAuth1Authenticator.ForProtectedResource(_key, _secret, _access, _accessSecret)
            };
            RestRequest request = new RestRequest($"1.1/statuses/show/{statusId}.json", Method.GET);
            IRestResponse response = client.Execute(request);
            return JsonConvert.DeserializeObject(response.Content) as JObject;
        }

        public bool ReactsToUrl(string url) {
            return url.Contains("twitter.com/") && url.Contains("status/");
        }
    }
}
