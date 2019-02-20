using System.Net.Sockets;

namespace Console.Plugins.Network.Server
{
    public class Client : BaseClient
    {
        public Client(TcpClient client)
        {
            Client = client;
            Stream = client?.GetStream();
        }
    }
}