using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Rabscuttle.handler;
using Rabscuttle.networking.commands;
using Rabscuttle.networking.io;
using Rabscuttle.plugins;

namespace FuckPlugin {
    [Export(typeof(IPluginContract))]
    public class FuckPlugin : IPluginContract {
        /// <summary> Gets or sets the name of the command. </summary>
        /// <value> The name of the command should be short and descriptive, usually a single word. </value>
        public string CommandName => "fuck";

        /// <summary> Gets or sets the back reference. </summary>
        /// <value> The back reference is a the plugin handler reference, to search for other plugins, for example. </value>
        public PluginHandler BackReference { get; set; }

        /// <summary> Gets or sets the rank. </summary>
        /// <value> The rank is the least amount of channel rights he has to have, otherwise the command execute will be ignored. </value>
        public MemberCode Rank => MemberCode.DEFAULT;

        /// <summary> Gets or sets the message prefix, which will be given by the bot. </summary>
        /// <value> The prefix is the kind of message prefix the bot usually listens to. </value>
        public string MessagePrefix { get; set; }

        /// <summary> Get or sets the help file, which the bot will use on request. </summary>
        /// <value> A helpfile, describing the plugins function, which might be sent by the bot on request. </value>
        public string HelpFile => "Just to fuck people.";

        /// <summary> Gets or sets the sender, which will be given by the bot. </summary>
        /// <value> A sender, which the plugin can send NetworkMessage to. </value>
        public ISender Sender { get; set; }

        /// <summary> Let the plugin subscribe to any of these Handlers - totally optional. </summary>
        /// <param name="handler">Any of the handlers, which will be given by the bot.</param>
        public void SubscribeTo(ObservableHandler handler) {
            //
        }

        private readonly string[] _phrases = {
            "Fuck off, {0} - {1}",
            "Fuck you, {0} - {1}",
            "{0} can go and fuck off. - {1}",
            "{0}, go and take a flying fuck at a rolling donut. - {1}",
            "{0}, Thou clay-brained guts, thou knotty-pated fool, thou whoreson obscene greasy tallow-catch! - {1}",
            "{0}, there aren't enough swear-words in the English language, so now I'll have to call you perkeleen vittupää just to express my disgust and frustration with this crap. - {1}",
            "Oh fuck off, just really fuck off you total dickface. Christ {0}, you are fucking thick. - {1}",
            "Fuck me gently with a chainsaw, {0}. Do I look like Mother Teresa? - {1}",
            "{0}, why don't you go outside and play hide-and-go-fuck-yourself? - {1}",
            "Fuck {0}. - {1}",
            "Fuck you very much. - {1}",
            "I don't give a flying fuck. - {1}",
            "Fascinating story, in what chapter do you shut the fuck up? - {1}",
            "What you've just said is one of the most insanely idiotic things I have ever heard, {0}. At no point in your rambling, incoherent response were you even close to anything that could be considered a rational thought. Everyone in this room is now dumber for having listened to it. I award you no points {0}, and may God have mercy on your soul. - {1}",
            "Cool story, Bro - {1}",
            "And {1} said unto {0}, \"Verily, cast thine eyes upon the field in which I grow my fucks\", and {0} gave witness unto the field, and saw that it was barren.",
            "Well {0}, aren't you a shining example of a rancid fuck-nugget. - {1}",
            "Fuck off, you must, {0}. - {1}",
            "Fucking {0} is a fucking pussy. I'm going to fucking bury that guy, I have done it before, and I will do it again. I'm going to fucking kill Arc Warden. - {1}",
            "I'd love to stop and chat to you but I'd rather have Type 2 diabetes. - {1}",
            "Christ on a bendy-bus, {0}, don't be such a fucking faff-arse. - {1}",
            "Merry Fucking Christmas, {0}. - {1}",
            "Happy Fucking Birthday, {0}. - {1}",
            "Come the fuck in or fuck the fuck off. - {1}",
            "Please choke on a bucket of cocks. - {1}",
            "Fuck you, your whole family, your pets, and your feces. - {1}",
            "{0}, shut the fuck up. - {1}",
            "You Fucktard! - {1}",
            "The point is, ladies and gentleman, that {0} -- for lack of a better word -- is good. {0} is right. {0} works. {0} clarifies, cuts through, and captures the essence of the evolutionary spirit. {0}, in all of its forms -- {0} for life, for money, for love, knowledge -- has marked the upward surge of mankind' - {1}"
        };

        /// <summary> Called when a private message was received by the bot. Careful: This can be in a channel too! </summary>
        /// <param name="message">The network message received.</param>
        public void OnPrivMsg(CommandMessage message) {
            if (String.IsNullOrEmpty(message.parameters)) {
                return;
            }
            var split = message.parameters.Split(new [] {' '}, 2)[0];

            var f = message.user.userName;
            var name = split;
            int num = new Random().Next(0, _phrases.Length);
            Sender.Send(RawPrivMsg.Generate(message.origin, String.Format(_phrases[num], name, f)));
        }

        /// <summary> Called when a notice message was received by the bot. </summary>
        /// <param name="message">The network message received.</param>
        public void OnNotice(CommandMessage message) {
        }
    }
}
