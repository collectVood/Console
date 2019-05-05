using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Console.Plugins.Network.Client
{
    public class Client : BaseClient
    {
        public Client(IPAddress address, int port)
        {
            try
            {
                Client = new TcpClient();
                try
                {
                    Client.Connect(address, port);
                }
                catch (SocketException)
                {
                    Log.Warning("Unable to connect to this server.");
                    return;
                } 

                Stream = Client?.GetStream();
                StartListening();
            }
            catch (Exception e)
            {
                Log.Exception(e);
            }
        }
    }
}