using System.Text.RegularExpressions;

namespace Rabscuttle.util {
    public class Validator {
        private static Regex channelRegex = new Regex(@"^\#[\w-]*$", RegexOptions.None);

        public static bool IsValidChannelName(string channel) {
            return channelRegex.IsMatch(channel);
        }
    }
}
