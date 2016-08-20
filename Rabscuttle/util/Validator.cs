using System.Text.RegularExpressions;

namespace Rabscuttle.util {
    public class Validator {
        private static readonly Regex CHANNEL_REGEX = new Regex(@"^\#[\w-]*$", RegexOptions.None);
        private static readonly Regex USER_ORIGIN_REGEX = new Regex(@"^(?<user>[\d\w-\[\]\\^_`{}|*]*)!(?<ident>[\d\w-\[\]\\^_`{}|~*]*)@(?<host>[\d\w-\[\]\\^_`{}|:\.*]*)$", RegexOptions.None);

        public static bool IsValidChannelName(string channel) {
            return CHANNEL_REGEX.IsMatch(channel);
        }

        public static MatchCollection UserOriginMatches(string origin) {
            return USER_ORIGIN_REGEX.Matches(origin);
        }
    }
}
