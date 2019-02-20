using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Console.Plugins.Network
{
    public class BaseClient
    {
        public TcpClient Client { get; protected set; }
        public NetworkStream Stream { get; protected set; }
        public StringBuilder Builder { get; protected set; } = new StringBuilder();

        public Events.OnNewMessage OnNewMessage = m => {};
        public Events.OnNewMessage OnMessageSent = m => {};

        public bool IsConnected => Client != null && Stream != null && Client.Connected;

        public void SendMessage(string message)
        {
            var msg = new BaseMessage(this, message);
            msg.Send(Stream);

            OnMessageSent(msg);
        }

        public void StartReceiving()
        {
            new Thread(() =>
            {
                while (true)
                {
                    var message = ReceiveMessage();
                    if (string.IsNullOrEmpty(message))
                        continue;
                    
                    OnNewMessage(new BaseMessage(this, message));
                }
                
                // ReSharper disable once FunctionNeverReturns
            }).Start();
        }

        private string ReceiveMessage()
        {
            if (!IsConnected)
                return null;
            
            var data = new byte[64];
            do
            {
                try
                {
                    var bytes = Stream.Read(data, 0, data.Length);
                    Builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                }
                catch (Exception)
                {
                    // ignored
                }

            } while (Stream.DataAvailable);

            var reply = Builder.ToString();
            Builder = Builder.Clear();
            return reply;
        }

        public void Close()
        {
            Stream?.Close();
            Client?.Close();
        }
    }
}