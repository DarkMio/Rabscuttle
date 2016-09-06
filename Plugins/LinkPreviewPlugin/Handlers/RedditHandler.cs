using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LinkPreviewPlugin.Handlers {
    class RedditHandler : ILinkHandler {

        private Regex _urlRegex = new Regex(@"(^https?://)?(\w+)?\.?(reddit\.com/|redd\.it/)(?<subreddit>r/\w+/)?(comments/)?(?<thread>\w+)(/.*/(?<comment>[\w|\d]+))?");

        public string GenerateResponse(string url) {
            var matches = _urlRegex.Matches(url)[0].Groups;
            string threadId = matches["thread"].Value;
            string commentId = matches["comment"].Value;
            JArray content = GetRedditThread(threadId, commentId);
            return BuildResponse(content, !String.IsNullOrWhiteSpace(commentId));
        }

        private JArray GetRedditThread(string threadId, string commentId) {
            string urlChunk = String.IsNullOrWhiteSpace(commentId) ? threadId : $"{threadId}/_/{commentId}";
            HttpWebRequest request = WebRequest.Create($"https://reddit.com/comments/{urlChunk}.json") as HttpWebRequest;
            request.Credentials = CredentialCache.DefaultCredentials;
            request.UserAgent = "Rabscuttle, an IRC Bot.";

            HttpWebResponse response = request.GetResponse() as HttpWebResponse;

            string responseContent = new StreamReader(response.GetResponseStream()).ReadToEnd();
            return JsonConvert.DeserializeObject(responseContent) as JArray;
        }

        private string BuildResponse(JArray content, bool isComment) {
            JObject threadData = content[0]
                                    .Value<JObject>()["data"]
                                    .Value<JObject>()["children"]
                                    .Value<JArray>()[0]
                                    .Value<JObject>()["data"]
                                    .Value<JObject>();

            string subreddit = threadData["subreddit"].Value<string>();
            string title = threadData["title"].Value<string>();
            long karma = threadData["score"].Value<long>();
            long commentCount = threadData["num_comments"].Value<long>();
            string response = $"/r/{subreddit}: {title} ({karma}👍 | {commentCount}📧)";
            if (isComment) {
                JObject commentData = content[1]
                                    .Value<JObject>()["data"]
                                    .Value<JObject>()["children"]
                                    .Value<JArray>()[0]
                                    .Value<JObject>()["data"]
                                    .Value<JObject>();

                string author = commentData["author"].Value<string>();
                string score = commentData["score"].Value<string>();
                response = $"/u/{author}s' comment ({score}👍) on " + response;
            }
            return response;
        }

        public bool ReactsToUrl(string url) {
            return url.Contains("redd.it") || url.Contains("reddit.com");
        }

    }
}
