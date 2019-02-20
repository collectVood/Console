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

        public void Send(TcpClient client) => Send(client?.GetStream());

        public void Send(NetworkStream stream)
        {
            if (_data == null || _data.Length <= 0)
                return;
            
            stream?.Write(_data, 0, _data.Length);
        }

        public string GetMessage() => Encoding.Unicode.GetString(_data);
    }
}