using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using System.Net;

namespace SuperPingPong.Networking
{
    public class NetworkManager
    {
        private GamerNetworkType networkRole;
        private NetPeerConfiguration configuration;
        public NetServer Server { get; set; }
        public NetClient Client { get; set; }      

        private const string IP = "127.0.0.1";
        private const string Port = "14242";
        
        public NetworkManager(GamerNetworkType networkRole)
        {
            this.networkRole = networkRole;
        }

        public void SetUpConnection()
        {
            configuration = new NetPeerConfiguration("PingPong");

            configuration.EnableMessageType(NetIncomingMessageType.WarningMessage);
            configuration.EnableMessageType(NetIncomingMessageType.VerboseDebugMessage);
            configuration.EnableMessageType(NetIncomingMessageType.ErrorMessage);
            configuration.EnableMessageType(NetIncomingMessageType.Error);
            configuration.EnableMessageType(NetIncomingMessageType.DebugMessage);
            configuration.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
            configuration.EnableMessageType(NetIncomingMessageType.Data);

            switch (networkRole)
            {
                case GamerNetworkType.Client:
                    Client = new NetClient(configuration);
                    Client.Start();
                    Client.Connect(new IPEndPoint(NetUtility.Resolve(IP), Convert.ToInt32(Port)));
                    break;
                case GamerNetworkType.Server:
                    configuration.Port = Convert.ToInt32(Port);
                    Server = new NetServer(configuration);
                    Server.Start();
                    break;
                default:
                    throw new ArgumentException("Network type was not set");
            }
        }

        public void SendToServer(GameMessageManager msg)
        {
            var om = this.Client.CreateMessage();
            msg.EncodeMessageToServer(om);
            this.Client.SendMessage(om, NetDeliveryMethod.ReliableUnordered);
        }

        public void SendToClient(GameMessageManager msg)
        {
            var om = this.Server.CreateMessage();
            msg.EncodeMessageToClient(om);
            this.Server.SendToAll(om, NetDeliveryMethod.ReliableUnordered);
        }
    }
}
