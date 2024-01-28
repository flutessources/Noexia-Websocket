namespace Noexia.Transport.WebSocket.Core.Processor
{
    public interface IMessageProcessor<T> where T : IMessageData
    {
        bool Process(T message, object[] objs, CancellationToken cancellationToken = default);
    }
}
