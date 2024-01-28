using Microsoft.AspNetCore.Http;
using Noexia.Transport.WebSocket.Core.Processor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Noexia.Transport.WebSocket.Core.Server
{
    public abstract class WebSocketServerBase
    {
        private List<WebsocketClientInstance> m_clients = new List<WebsocketClientInstance>();

        public WebSocketServerBase()
        {
            MessageProcessorService.Init(GetAssemblies());
        }

        protected abstract Assembly[] GetAssemblies();

        public async Task OnClientConnect(HttpContext context, System.Net.WebSockets.WebSocket webSocket)
        {
            Console.WriteLine("On Client Connect");

            WebsocketClientInstance client = new WebsocketClientInstance(webSocket, 1024 * 4);
            client.onClientDisconnect += OnClientDisconnect;
            m_clients.Add(client);
            OnClientConnected(client);
            await client.ListenAsync();
        }

        protected virtual void OnClientConnected(WebsocketClientInstance client)
        {
        }

        private void OnClientDisconnect(WebsocketClientInstance client)
        {
            client.onClientDisconnect -= OnClientDisconnect;
            m_clients.Remove(client);

            OnClientDisconnected(client);
        }

        protected virtual void OnClientDisconnected(WebsocketClientInstance client)
        {
        }
    }
}
