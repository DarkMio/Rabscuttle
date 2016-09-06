using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Rabscuttle.handler;
using Rabscuttle.networking.commands;
using Rabscuttle.networking.io;
using Rabscuttle.plugin;

namespace QuotePlugin {
    public class QuotePlugin : IPluginContract {
        public string CommandName => "quote";
        public PluginHandler BackReference { get; set; }
        public MemberCode Rank => MemberCode.DEFAULT;
        public string MessagePrefix { get; set; }
        private static readonly QuoteManager MANAGER = QuoteManager.Instance;

        public string HelpFile => "Adds, deletes or throws out some quotes. Commands are:\n" +
                                  $"{MessagePrefix}quote: Prints out a random quote.\n" +
                                  $"{MessagePrefix}quote [index]: Prints out a certain quote.\n" +
                                  $"{MessagePrefix}quote delete [index]: Deletes the quote with the index\n" +
                                  $"{MessagePrefix}quote [quote text]: Adds the given text to the quote storage.\n" +
                                  $"{MessagePrefix}quote status: Shows how many quotes there are and how many people contributed to it.\n" +
                                  $"{MessagePrefix}quote defrag: Defragments the quote index-numbers of every quote. Operators only.\n" +
                                  $"{MessagePrefix}quote render: Moves the storage to the website to force a re-render. Operators only.";
        public ISender Sender { get; set; }
        public void SubscribeTo(ObservableHandler handler) { }
        public void OnPrivMsg(CommandMessage message) {
            string[] parameters = message.parameters.Split();
            bool debugInfo = false;
            // check if the last statement is an --info parameter (with additional debugging data)
            if (parameters.Length > 0 && parameters.Last() == "--info") {
                debugInfo = true;
                parameters = parameters.Take(parameters.Length - 1).ToArray();
            }

            // if there's no additional parameters throw out a random element.
            if (parameters.Length < 2 && String.IsNullOrWhiteSpace(parameters[0])) {
                SendQuote(MANAGER.GetRandomQuote(), message.origin, debugInfo);
                return;
            }

            // otherwise, parse the first parameter as long - if its successful, output a quote with that ID
            long isDigit = 0;
            if (parameters.Length >= 0 && long.TryParse(parameters[0], out isDigit)) {
                JArray array = MANAGER.GetQuote(isDigit);
                if (array != null) { // was found, output it
                    SendQuote(array, message.origin, debugInfo);
                } else { // send an info that it doesnt exist
                    Sender.Send(RawPrivMsg.Generate(message.origin, $"There exists no quote with index {isDigit}"));
                }
                return;
            }

            // the index page reply
            if (parameters.Length >= 0 && parameters[0] == "index") {
                Sender.Send(RawPrivMsg.Generate(message.origin, "The index is found at: http://moddota.com/rabscuttle/"));
                return;
            }

            // rendering the json is now just writing out the json.
            if (parameters.Length >= 0 && parameters[0] == "render") {
                if (message.permission >= MemberCode.BOTOPERATOR) {
                    MANAGER.WriteOut();
                    Sender.Send(RawNotice.Generate(message.user.userName, "JSON was rewritten."));
                } else {
                    Sender.Send(RawNotice.Generate(message.user.userName, "Sorry, you're not allowed to do that."));
                }
                return;
            }

            // defragging resorts the index numbers increasingly, so IDs arent too far away from the count.
            if (parameters.Length >= 0 && parameters[0] == "defrag") {
                if (message.permission >= MemberCode.BOTOPERATOR) {
                    MANAGER.DefragmentQuotes();
                    Sender.Send(RawNotice.Generate(message.user.userName, "Defragmented the quote index numbers"));
                } else {
                    Sender.Send(RawNotice.Generate(message.user.userName, "Sorry, you're not allowed to do that."));
                }
                return;
            }

            // deletetion needs two parameters and needs bot operator rights
            if (parameters.Length >= 1 && parameters[0] == "delete") {
                if (parameters.Length < 2 || !long.TryParse(parameters[1], out isDigit)) {
                    Sender.Send(RawNotice.Generate(message.user.userName, "The command input seems to be malformed," +
                                                                          $" try: {MessagePrefix}quote delete [INTEGER/LONG]"));
                    return;
                }
                if (message.permission >= MemberCode.BOTOPERATOR) {
                    JArray quote = MANAGER.GetQuote(isDigit);
                    MANAGER.RemoveQuote(isDigit);
                    Sender.Send(RawPrivMsg.Generate(message.origin, $"Deleted this quote: {quote[1].Value<string>()}"));
                } else {
                    Sender.Send(RawNotice.Generate(message.user.userName, "Sorry, you're not allowed to do that."));
                }

                return;
            }

            // if neither of these things forced a return, then we're supposed to add a quote
            long indexNumber = MANAGER.AddQuote(message.parameters, message.user);
            Sender.Send(RawPrivMsg.Generate(message.origin, $"Your quote has been added, its index is {indexNumber}."));
        }
        public void OnNotice(CommandMessage message) { }

        /// <summary>Uses a raw QuoteManager element, formats it and sends it to the channel</summary>
        private void SendQuote(JArray element, string channel, bool info = false) {
            if (element == null) {
                return;
            }
            // a quote always formats this way
            string format = $"{element[0].Value<long>()}: {element[1].Value<string>()} ";
            // and additionally, if the info-output was expected, add this part:
            if(info) {
                DateTime dtDateTime = new DateTime(1970,1,1,0,0,0,0,System.DateTimeKind.Utc);
                dtDateTime = dtDateTime.AddSeconds(element[3].Value<double>()).ToLocalTime();
                format += $"| submitted by {element[2].Value<string>()} at {dtDateTime}";
            }
            Sender.Send(RawPrivMsg.Generate(channel, format));
        }
    }
}
