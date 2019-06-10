using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;

namespace Console.Plugins.Network.Server
{
    public class BaseServer
    {
        public TcpListener Listener { get; }
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
                while (IsInitialized)
                {
                    HandleNewClient(Listener?.AcceptTcpClient());
                }
            });
            
            ListenThread.Start();
        }

        /// <summary>
        /// Handles new TCP client
        /// </summary>
        /// <param name="tcpClient">TCP client</param>
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

        /// <summary>
        /// Closes all connections and stops the server
        /// </summary>
        public void Close()
        {
            var clientsCount = Clients.Count;
            for (var i = 0; i < clientsCount; i++)
            {
                Clients[i]?.Close();
            }

            Listener?.Stop();
        }

        /// <summary>
        /// Returns true if the listener is active
        /// </summary>
        /// <returns>True if the listener is active</returns>
        public bool IsListenerActive()
        {
            return Listener?.GetType().GetProperty("Active", BindingFlags.Instance | BindingFlags.NonPublic)
                       ?.GetValue(Listener) is bool result && result;
        }
    }
}