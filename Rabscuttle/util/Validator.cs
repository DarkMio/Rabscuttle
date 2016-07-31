using System.Text.RegularExpressions;

namespace Rabscuttle.util {
    public class Validator {
        private static readonly Regex CHANNEL_REGEX = new Regex(@"^\#[\w-]*$", RegexOptions.None);

        public static bool IsValidChannelName(string channel) {
            return CHANNEL_REGEX.IsMatch(channel);
        }
    }
}
