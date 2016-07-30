using System;
using System.Diagnostics;
using Rabscuttle.core;
using Rabscuttle.core.commands;
using Rabscuttle.core.io;
using Rabscuttle.stuff;
using Rabscuttle.util;

namespace Rabscuttle {
    public class Rabscuttle {
        private readonly ConnectionManager cmngr;


        public static void Main(string[] args) {
            ConfigurationProvider.Bootstrap();
            new Rabscuttle().Run();

        }

        public Rabscuttle() {
            ConfigurationProvider.Bootstrap();
            Logger.Setup();
            Logger.WriteInfo("Rabscuttle", "Building up a connection.");
            string network = ConfigurationProvider.Get("network");
            string port = ConfigurationProvider.Get("port");
            if (String.IsNullOrEmpty(network)) {
                Logger.WriteFatal("NETWORK>", network);
                Logger.WriteFatal("Rabscuttle", "No network in configuration, please consult Rabscuttle.exe.config");
                throw new ArgumentException("No network in configuration, please consult Rabscuttle.exe.config");
            }

            int portNr;
            if (!Int32.TryParse(port, out portNr)) {
                Logger.WriteWarn("Rabscuttle", "Network port [ {0} ] is not a valid number, fallback on 6667", port);
                portNr = 6667;
            }

            cmngr = new ConnectionManager(network, portNr);
            Logger.WriteInfo("Rabscuttle", "Connection ready.");
        }

        public void Run() {
            BootCommands();
            Join();
            while (true) {
                cmngr.ReceiveUntil(CommandCode.PING);
            }
        }

        private void BootCommands() {
            string commandString = ConfigurationProvider.Get("bootcommands");
            string[] commands = commandString.Split(',');
            foreach (string command in commands) {
                string rawCommand = command.Trim();
                if (String.IsNullOrEmpty(rawCommand)) {
                    continue;
                }
                NetworkMessage nmsg = new NetworkMessage(rawCommand, false);
                cmngr.Send(nmsg);
            }
        }

        private void Join() {
            string channelString = ConfigurationProvider.Get("channels");
            string[] channels = channelString.Split(',');
            foreach (string channel in channels) {
                string qualifier = channel.Trim();
                if (Validator.IsValidChannelName(qualifier)) {
                    cmngr.Send(RawJoin.Generate(qualifier));
                } else {
                    Logger.WriteWarn("Rabscuttle", "Channel in configuration does not appear to be valid: {0}", qualifier);
                }
            }
        }
    }
}
