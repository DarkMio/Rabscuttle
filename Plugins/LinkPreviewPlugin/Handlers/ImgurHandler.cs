using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Imgur.API.Authentication.Impl;
using Imgur.API.Endpoints.Impl;
using RestSharp;
using RestSharp.Authenticators;

namespace LinkPreviewPlugin.Handlers {
    public class ImgurHandler : ILinkHandler {
        private const string Path = "../Plugins/PluginData/";
        private readonly string _key;
        private readonly string _secret;
        private readonly Regex _regex = new Regex(@"(?:
                                                      ^(?:http|https):\/\/
                                                    )?
                                                    (?:i\.)?
                                                    imgur.com\/
                                                    (?:
                                                      (?:gallery\/)(?<galleryid>\w+)|
                                                      (?:a\/)(?<albumid>\w+)\#?|
                                                      (?:topic\/)(?<topicid>)\w+\/
                                                    )?
                                                    (?<imgid>\w*)", RegexOptions.IgnorePatternWhitespace);

        public ImgurHandler() {
            using (StreamReader s = new StreamReader(Path + "imgur.key")) {
                _key = s.ReadLine();
                _secret = s.ReadLine();
            }
        }

        private static string FormatFilesize(double number) {
            return FormatAny(number, 1024.0, 2, "B");
        }

        private static string FormatViews(double number) {
            return FormatAny(number, 1000.0);
        }

        private static string FormatAny(double number, double divisor, int formatLen = 0, string postScript = "") {
            int count = 0;
            string[] formatter = {"", "K", "M", "G", "T", "E"};
            // decrement by thousand, while incrementing the counter
            while (number > divisor) {
                number /= divisor;
                count++;
            }
            return $"{number:N}{formatter[count]}{postScript}";
        }

        public string GenerateResponse(string url) {
            var matches = _regex.Matches(url)[0].Groups;
            var client = new ImgurClient(_key, _secret);
            string albumId = matches["albumid"].Value;
            string galleryId = matches["galleryid"].Value;
            string imageId = matches["imgid"].Value;
            if (!string.IsNullOrWhiteSpace(albumId) || !string.IsNullOrWhiteSpace(galleryId)) {
                var endpoint = new AlbumEndpoint(client);
                var relevantId = string.IsNullOrWhiteSpace(albumId) ? galleryId : albumId;
                var result = endpoint.GetAlbumAsync(relevantId).Result;
                var images = result.ImagesCount;
                var views = FormatViews(result.Views);
                return $"imgur: {result.Title + " "}(📷{images} | 👁{views})";
            }

            if (!string.IsNullOrWhiteSpace(imageId)) {
                var endpoint = new ImageEndpoint(client);
                var result = endpoint.GetImageAsync(imageId).Result;
                var size = FormatFilesize(result.Size);
                var views = FormatViews(result.Views);
                var traffic = FormatFilesize(result.Bandwidth);
                return $"imgur: {result.Title + " "}(💿{size} | 👁{views} | 🔌{traffic})";
            }

            return null;
        }

        public bool ReactsToUrl(string url) {
            return url.Contains("imgur.com");
        }
    }
}
