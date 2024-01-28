using System.Net.WebSockets;
using System.Text;
using Noexia.Transport.WebSocket.Core.Processor;

namespace Noexia.Transport.WebSocket.Core.Server
{
    public class WebsocketClientInstance
    {
        private readonly System.Net.WebSockets.WebSocket m_socket;

        private readonly CancellationTokenSource m_receiveCancellationTokenSource = new CancellationTokenSource();
        private readonly CancellationTokenSource m_sendCancellationTokenSource = new CancellationTokenSource();
        private readonly CancellationTokenSource m_disconnectionCancellationTokenSource = new CancellationTokenSource();

        private readonly CancellationToken m_receiveCancellationToken;
        private readonly CancellationToken m_sendCancellationToken;
        private readonly CancellationToken m_disconnectionCancellationToken;

        private uint m_bufferLength;
        public bool IsConnected { get; private set; }

        public Action<WebsocketClientInstance>? onClientDisconnect;

        public WebsocketClientInstance(System.Net.WebSockets.WebSocket socket, uint bufferLength)
        {
            m_socket = socket;
            m_bufferLength = bufferLength;
            m_receiveCancellationToken = m_receiveCancellationTokenSource.Token;
            m_sendCancellationToken = m_sendCancellationTokenSource.Token;
            m_disconnectionCancellationToken = m_disconnectionCancellationTokenSource.Token;
        }

        public async Task ListenAsync()
        {
            var buffer = new byte[m_bufferLength];

            while (m_socket.State == WebSocketState.Open)
            {
                var segment = new ArraySegment<byte>(buffer);

                if (segment.Array == null)
                    throw new Exception("Buffer is null");

                WebSocketReceiveResult result;
                using (var ms = new MemoryStream())
                {
                    do
                    {
                        result = await m_socket.ReceiveAsync(segment, m_sendCancellationToken);

                        try
                        {
                            await ms.WriteAsync(segment.Array, segment.Offset, result.Count);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            await DisconnectClientAsync(WebSocketCloseStatus.InternalServerError, e.Message);
                            return;
                        }
                    }
                    while (!result.EndOfMessage);

                    try
                    {
                        ms.Seek(0, SeekOrigin.Begin);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        await DisconnectClientAsync(WebSocketCloseStatus.InternalServerError, e.Message);
                        return;
                    }

                    if (result.MessageType == WebSocketMessageType.Binary)
                    {
                        byte[] messageData = ms.ToArray();

                        HandleMessage(messageData);
                    }
                    else if (result.MessageType == WebSocketMessageType.Text)
                    {
                        string messageData = Encoding.UTF8.GetString(ms.ToArray());

                        HandleMessage(messageData);
                    }
                    else if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await DisconnectClientAsync(WebSocketCloseStatus.NormalClosure, "");
                        return;
                    }
                }
            }

            await DisconnectClientAsync(WebSocketCloseStatus.NormalClosure, "");
        }

        private async Task DisconnectClientAsync(WebSocketCloseStatus closeStatus, string message)
        {
            await CloseSocketAsync(closeStatus, message);

            m_socket.Dispose();
            OnClientDisconnect();
        }

        private void OnClientDisconnect()
        {
            // Cancel all running processes if needed
            m_disconnectionCancellationTokenSource.Cancel();

            onClientDisconnect?.Invoke(this);
            IsConnected = false;
        }

        public async Task SendMessageAsync(byte[] bytes)
        {
            await m_socket.SendAsync(bytes, WebSocketMessageType.Binary, true, CancellationToken.None);
        }

        public async Task SendMessageAsync(string text)
        {
            ArraySegment<byte> bytes = new ArraySegment<byte>(Encoding.UTF8.GetBytes(text));
            await m_socket.SendAsync(bytes, WebSocketMessageType.Text, true, CancellationToken.None);
        }

        private void HandleMessage(byte[] data)
        {
            var result = MessageProcessorService.ProcessMessage(data, new object[1] { this }, m_disconnectionCancellationToken);
            if (result.Success == false)
            {
                Console.WriteLine($"Error on process the message {result.Id}");
            }
            else
            {
                Console.WriteLine($"Message {result.Id} processed");
            }
        }

        private void HandleMessage(string data)
        {
            var result = MessageProcessorService.ProcessMessage(data, new object[1] { this }, m_disconnectionCancellationToken);
            if (result.Success == false)
            {
                Console.WriteLine($"Error on process the message {result.Id}");
            }
            else
            {
                Console.WriteLine($"Message {result.Id} processed");
            }
        }

        private async Task CloseSocketAsync(WebSocketCloseStatus status, string? description)
        {
            await m_socket.CloseAsync(status, description, CancellationToken.None);
        }
    }
}
