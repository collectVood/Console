using System.Net.Sockets;
using System.Text;

namespace Console.Plugins.Network
{
    public class BaseMessage
    {
        public BaseClient Sender { get; }
        
        private byte[] _data;

        public BaseMessage(BaseClient sender, string message)
        {
            Sender = sender;
            _data = Encoding.Unicode.GetBytes(message);
        }

        /// <summary>
        /// Send message to the specified TCP client
        /// </summary>
        /// <param name="client">TCP client</param>
        public void Send(TcpClient client) => Send(client?.GetStream());

        /// <summary>
        /// Send message in the specified network stream
        /// </summary>
        /// <param name="stream">Network stream</param>
        public void Send(NetworkStream stream)
        {
            if (_data == null || _data.Length <= 0)
                return;
            
            stream?.Write(_data, 0, _data.Length);
        }

        /// <summary>
        /// Get message from data array
        /// </summary>
        /// <returns>Message</returns>
        public string GetMessage() => Encoding.Unicode.GetString(_data);
    }
}