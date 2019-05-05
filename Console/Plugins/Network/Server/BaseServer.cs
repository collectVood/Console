using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;

namespace Console.Plugins.Network.Server
{
    public class BaseServer
    {
        public TcpListener Listener { get; private set; }
        public List<BaseClient> Clients { get; } = new List<BaseClient>();
        public bool IsInitialized => IsListenerActive();

        public event Events.OnNewClient OnNewClient = c => {};
        public event Events.OnNewMessage OnNewMessage = c => {};

        public Thread ListenThread;

        public BaseServer(IPAddress address, int port)
        {
            if (IsInitialized)
                return;
            
            Listener = new TcpListener(address, port);
            Listener.Start();

            ListenThread = new Thread(() =>
            {
                while (true)
                {
                    HandleNewClient(Listener?.AcceptTcpClient());
                }
                
                // ReSharper disable once FunctionNeverReturns
            });
            
            ListenThread.Start();
        }

        private void HandleNewClient(TcpClient tcpClient)
        {
            var stream = tcpClient?.GetStream();
            if (stream == null)
                return;
            
            var client = new Client(tcpClient);
            Clients.Add(client);

            client.OnNewMessage += m => OnNewMessage(m);
            client.StartListening();
            OnNewClient(client);
        }

        public void Close()
        {
            var clientsCount = Clients.Count;
            for (var i = 0; i < clientsCount; i++)
            {
                Clients[i]?.Close();
            }

            ListenThread.Abort();
            Listener?.Stop();
        }

        public bool IsListenerActive()
        {
            return Listener?.GetType().GetProperty("Active", BindingFlags.Instance | BindingFlags.NonPublic)
                       ?.GetValue(Listener) is bool result && result;
        }
    }
}