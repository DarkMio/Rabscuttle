using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Rabscuttle.util {
    public class JsonUtility {

        public const string PluginPath = "../Plugins/PluginData/";

        public static T ReadAndParseFile<T>(string path, string filename) {
            try {
                using (StreamReader s = new StreamReader(path + filename)) {
                    string json = s.ReadToEnd();
                    return (T) JsonConvert.DeserializeObject(json);
                }
            } catch (IOException e) {
                Logger.WriteWarn("Quote Manager", $"Something went wrong while reading: {e}");
                return default(T);
            }
        }

        public static void SerializeAndWriteFile<T>(T data, string path, string filename) {
            if (data == null) {
                data = default(T);
            }

            string output = JsonConvert.SerializeObject(data);
            using (StreamWriter s = new StreamWriter(path + filename)) {
                s.Write(output);
            }
        }
    }
}
