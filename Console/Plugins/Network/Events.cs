namespace Console.Plugins.Network
{
    public class Events
    {
        public delegate void OnNewClient(Server.Client client);

        public delegate void OnNewMessage(BaseMessage message);
    }
}