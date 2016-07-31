using System;
using System.Diagnostics;
using Rabscuttle.networking;
using Rabscuttle.networking.commands;
using Rabscuttle.networking.io;
using Rabscuttle.stuff;
using Rabscuttle.util;

namespace Rabscuttle {
    public class Rabscuttle : IDisposable {
        private readonly ConnectionManager _connectionManager;


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
                Logger.WriteFatal("Rabscuttle", "No network in configuration, please consult Rabscuttle.exe.config");
                throw new ArgumentException("No network in configuration, please consult Rabscuttle.exe.config");
            }

            int portNr;
            if (!Int32.TryParse(port, out portNr)) {
                Logger.WriteWarn("Rabscuttle", "Network port [ {0} ] is not a valid number, fallback on 6667", port);
                portNr = 6667;
            }

            _connectionManager = new ConnectionManager(network, portNr);
            Logger.WriteInfo("Rabscuttle", "Connection ready.");
        }

        public void Run() {
            BootCommands();
            Join();
            while (true) {
                _connectionManager.ReceiveUntil(CommandCode.PING);
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
                _connectionManager.Send(nmsg);
            }
        }

        private void Join() {
            string channelString = ConfigurationProvider.Get("channels");
            string[] channels = channelString.Split(',');
            foreach (string channel in channels) {
                string qualifier = channel.Trim();
                if (Validator.IsValidChannelName(qualifier)) {
                    _connectionManager.Send(RawJoin.Generate(qualifier));
                } else {
                    Logger.WriteWarn("Rabscuttle", "Channel in configuration does not appear to be valid: {0}", qualifier);
                }
            }
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposable) {
            if (!disposable) {
                return;
            }
            _connectionManager?.Dispose();
        }
    }
}
