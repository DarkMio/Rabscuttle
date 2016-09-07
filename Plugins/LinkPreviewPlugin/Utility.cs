using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LinkPreviewPlugin {
    public class Utility {

        public static T GetFromApi<T>(string url) {
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Credentials = CredentialCache.DefaultCredentials;
            request.UserAgent = "Rabscuttle, an IRC Bot.";

            HttpWebResponse response = request.GetResponse() as HttpWebResponse;

            string responseContent = new StreamReader(response.GetResponseStream()).ReadToEnd();
            return JsonConvert.DeserializeObject<T>(responseContent);
        }
    }
}
