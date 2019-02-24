// ReSharper disable InconsistentNaming

using System.Net;
using Console.Plugins.Commands;
using Console.Plugins.Hooks;
using Console.Plugins.Network;
using Console.Plugins.Network.Server;
using Client = Console.Plugins.Network.Client.Client;

namespace Console.Plugins.Core
{
    [Info("Chat", "Iv Misticos", "1.0.0")]
    public class Chat : Plugin
    {
        #region Variables
        
        private Client _client;
        private BaseServer _server;
        
        #endregion
        
        #region Hooks

        [HookMethod("CanInput")]
        public object CanInput(string entry)
        {
            if (entry.StartsWith("/") || _client?.IsConnected != true)
                return null;
            
            _client.SendMessage(entry);
            return false;
        }
        
        #endregion
        
        #region Commands
        
        [Command("run")]
        public void CommandRun(CommandArgument arg)
        {
            if (arg.Args.Length != 3)
            {
                arg.Reply("Please, use S (server) or C (client), IP and Port");
                return;
            }

            if (!IPAddress.TryParse(arg.Args[1], out var address))
            {
                arg.Reply("Please, specify correct IP");
                return;
            }

            if (!int.TryParse(arg.Args[2], out var port))
            {
                arg.Reply("Please, specify correct Port");
                return;
            }
            
            switch (arg.Args[0].ToLower())
            {
                case "s":
                case "server":
                {
                    if (_server?.IsInitialized == true)
                    {
                        arg.Reply("Server is already initialized. Please, stop it first.");
                        return;
                    }
                    
                    _server = new BaseServer(address, port);
                    _server.OnNewMessage += OnMessage;
                    break;
                }
                
                case "c":
                case "client":
                {
                    if (_client?.IsConnected == true)
                    {
                        arg.Reply("Client is already initialized. Please, stop it first");
                        return;
                    }
                    
                    _client = new Client(address, port);
                    _client.OnNewMessage += OnMessage;
                    break;
                }

                default:
                {
                    arg.Reply("Please, use S (server) or C (client).");
                    return;
                }
            }
            
            arg.Reply("Done.");
        }
        
        [Command("stop")]
        public void CommandStop(CommandArgument arg)
        {
            if (arg.Args.Length != 1)
            {
                arg.Reply("Please, use S (server) or C (client)");
                return;
            }
            
            switch (arg.Args[0].ToLower())
            {
                case "s":
                case "server":
                {
                    if (_server == null || !_server.IsInitialized)
                    {
                        arg.Reply("Server is not running");
                        return;
                    }
                    
                    _server.Close();
                    break;
                }
                
                case "c":
                case "client":
                {
                    if (_client == null || !_client.IsConnected)
                    {
                        arg.Reply("Client is not running");
                        return;
                    }

                    _client.Close();
                    break;
                }

                default:
                {
                    arg.Reply("Please, use S (server) or C (client).");
                    return;
                }
            }
            
            arg.Reply("Done.");
        }

        [Command("status")]
        public void CommandStatus(CommandArgument arg)
        {
            var server = _server?.IsInitialized == true ? "RUNNING" : "STOPPED";
            var client = _client?.IsConnected == true ? "RUNNING" : "STOPPED";
            
            arg.Reply($"STATUS\n" +
                      $"Server: {server}\n" +
                      $"Client: {client}");
        }

        [Command("send")]
        public void CommandSend(CommandArgument arg)
        {
            if (arg.Args.Length <= 0)
            {
                arg.Reply("Please, specify the message");
                return;
            }

            _client.SendMessage(arg.Arguments);
        }
        
        #endregion
        
        #region Message Handling

        public void OnMessage(BaseMessage msg)
        {
            if (msg.Sender == _client)
                return;
            
            var message = msg.GetMessage();
            Log.Warning($"NEW MESSAGE: {message}");
        }
        
        #endregion
    }
}