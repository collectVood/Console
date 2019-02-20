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
                Client.Connect(address, port);
                Stream = Client?.GetStream();
                StartReceiving();
            }
            catch (Exception e)
            {
                Log.Exception(e);
            }
        }
    }
}